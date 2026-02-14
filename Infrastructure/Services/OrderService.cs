using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaaSApp.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly TenantContext _tenantContext;
        private readonly IConfiguration _config;
        private readonly IPaymentGatewayFactory _gatewayFactory;
        private readonly IMyFatoorahService _myFatoorah;

        public OrderService(
            AppDbContext context,
            TenantContext tenantContext,
            IConfiguration config,
            IPaymentGatewayFactory gatewayFactory,
            IMyFatoorahService myFatoorah)
        {
            _context = context;
            _tenantContext = tenantContext;
            _config = config;
            _gatewayFactory = gatewayFactory;
            _myFatoorah = myFatoorah;
        }

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            var tenantId = _tenantContext.TenantId;
            var order = new Order
            {
                TenantId = tenantId,
                CustomerName = request.CustomerName,
                Email = request.Email,
                Mobile = request.Mobile,
                Mode = request.Mode,
                Total = request.Total,
                Status = "Pending",
                Items = request.Items.Select(i => new OrderItem
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reduce stock for all order items
            foreach (var orderItem in order.Items)
            {
                if (orderItem.ItemId.HasValue)
                {
                    var menuItem = await _context.Items
                        .Where(x => x.Id == orderItem.ItemId.Value && x.TenantId == tenantId)
                        .FirstOrDefaultAsync();

                    if (menuItem != null && menuItem.IsTrackStock)
                    {
                        menuItem.StockQuantity -= orderItem.Quantity;

                        if (menuItem.StockQuantity <= 0)
                        {
                            menuItem.StockQuantity = 0;
                            menuItem.IsAvailable = false;
                        }
                    }
                }
            }

            // Save stock changes once after processing all items
            await _context.SaveChangesAsync();

            return order.Id;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.TenantId == _tenantContext.TenantId);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Email = order.Email,
                Mobile = order.Mobile,
                Mode = order.Mode,
                Total = order.Total,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> GetByCustomerMobileAsync(string mobile)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Mobile == mobile && o.TenantId == _tenantContext.TenantId)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                Email = o.Email,
                Mobile = o.Mobile,
                Mode = o.Mode,
                Total = o.Total,
                Status = o.Status,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            });
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.TenantId == _tenantContext.TenantId)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == _tenantContext.TenantId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<string> CreateOrderPaymentLinkAsync(int orderId)
        {
            var tenantId = _tenantContext.TenantId;
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenantId);

            if (order == null)
                throw new Exception("Order not found");

            var gateway = _gatewayFactory.Resolve(tenantId);

            var payment = new PaymentRequest
            {
                Amount = order.Total,
                CustomerEmail = order.Email,
                CustomerName = order.CustomerName,
                CustomerReference = order.Id.ToString(),
                CallbackUrl = _config["Payment:OrderCallbackUrl"],
                ErrorUrl = _config["Payment:ErrorUrl"]
            };

            var link = await gateway.CreatePaymentLink(payment);

            order.PaymentStatus = "Pending";
            order.InvoiceId = link.InvoiceId;

            await _context.SaveChangesAsync();

            return link.PaymentUrl;
        }


        //old version happy senario
        public async Task<bool> HandleOrderCallbackAsync1(string paymentId)
        {
            var result = await _myFatoorah.GetPaymentStatus(paymentId);

            if (result.Data.InvoiceStatus != "Paid")
                return false;

            int orderId = int.Parse(result.Data.CustomerReference);

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.PaymentStatus = "Paid";
            order.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<OrderCallbackResult> HandleOrderCallbackAsync(string paymentId)
        {
            var status = await _myFatoorah.GetPaymentStatus(paymentId);

            if (status == null || status.Data == null)
                return OrderCallbackResult.Failed();

            // 1. Check invoice status
            if (status.Data.InvoiceStatus != "Paid")
                return OrderCallbackResult.Failed();

            // 2. Extract order ID
            int orderId = int.Parse(status.Data.CustomerReference);

            var order = await _context.Orders
                .Include(o => o.Tenant)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return OrderCallbackResult.Failed();

            // 3. Idempotency check
            if (order.PaymentStatus == "Paid")
                return OrderCallbackResult.AlreadyProcessed(order);

            // 4. Update order
            order.PaymentStatus = "Paid";
            order.Status = "Confirmed";
            order.InvoiceId ??= status.Data.InvoiceId.ToString();

            await _context.SaveChangesAsync();

            return OrderCallbackResult.Success(order);
        }

        public async Task ProcessWebhookAsync(PaymentWebhookEvent webhook)
        {
            var orderId = int.Parse(webhook.ReferenceId.Replace("ORDER-", ""));

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return;

            // 🔐 Transition Guard
            if (!IsValidOrderTransition(order.Status, webhook.EventType))
            {
                //_logger.LogWarning(
                //    "Invalid order state transition blocked | Order={OrderId} Current={CurrentStatus} Incoming={IncomingEvent}",
                //    order.Id,
                //    order.Status,
                //    webhook.EventType
                //);
                return;
            }

            switch (webhook.EventType)
            {
                case "PaymentPaid":

                    if (order.Status != "Completed")
                    {
                        order.Status = "Completed";
                        order.PaidAt ??= webhook.OccurredAt;
                    }

                    break;

                case "PaymentFailed":
                case "PaymentExpired":

                    // Only update if not already completed
                    if (order.Status != "Completed")
                    {
                        order.Status = "Failed";
                    }

                    break;

                case "PaymentRefunded":

                    if (order.Status == "Completed")
                    {
                        order.Status = "Refunded";
                    }

                    break;
            }


            await _context.SaveChangesAsync();
        }


        private bool IsValidOrderTransition(string currentStatus, string incomingEvent)
        {
            if (currentStatus == "Completed")
                return false; // Completed orders never change

            if (currentStatus == "Failed" && incomingEvent == "PaymentFailed")
                return false;

            if (currentStatus == "Expired" && incomingEvent == "PaymentExpired")
                return false;

            return true;
        }





    }
}
