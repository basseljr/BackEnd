using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly AppDbContext _db;
        private readonly IServiceProvider _provider;

        public PaymentGatewayFactory(AppDbContext db, IServiceProvider provider)
        {
            _db = db;
            _provider = provider;
        }

        public IPaymentGateway Resolve(int tenantId)
        {
            var settings = _db.TenantPaymentSettings.FirstOrDefault(s => s.TenantId == tenantId)
                ?? throw new Exception("Tenant has not configured a payment gateway!");

            return settings.GatewayTypeId switch
            {
                1 => _provider.GetRequiredService<MyFatoorahGateway>(),
                // 2 => _provider.GetRequiredService<TapGateway>(),
                // 3 => _provider.GetRequiredService<StripeGateway>(),
                _ => throw new Exception("Unsupported gateway.")
            };
        }
    }
}
