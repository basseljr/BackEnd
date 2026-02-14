using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MyFatoorahGateway : IPaymentGateway
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public MyFatoorahGateway(
            HttpClient http,
            IConfiguration config)
        {
            _http = http;
            _apiKey = config["MyFatoorah:ApiKey"];
            _baseUrl = config["MyFatoorah:BaseUrl"];
        }

        public async Task<PaymentLinkResponse> CreatePaymentLink(PaymentRequest request)
        {
            var body = new
            {
                PaymentMethodId = 2,
                InvoiceValue = request.Amount,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerReference = request.CustomerReference,
                CallBackUrl = request.CallbackUrl,
                ErrorUrl = request.ErrorUrl
            };

            var httpReq = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v2/ExecutePayment");
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            httpReq.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var res = await _http.SendAsync(httpReq);
            var json = await res.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return new PaymentLinkResponse
            {
                InvoiceId = "232323",/*data.Data.InvoiceId.ToString(),*/
                PaymentUrl = "https://aiw.com"/*data.Data.PaymentURL.ToString()*/
            };
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(string paymentId)
        {
            // Your existing MF logic here…
            return null;
        }
    }
}
