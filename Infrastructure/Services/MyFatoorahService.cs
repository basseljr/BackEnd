using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Services
{
    public class MyFatoorahService : IMyFatoorahService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public MyFatoorahService(IConfiguration config)
        {
            _apiKey = config["MyFatoorah:ApiKey"];
            _baseUrl = config["MyFatoorah:BaseUrl"];
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
            return client;
        }

        public async Task<string> InitiatePayment(decimal amount, string currency = "KWD")
        {
            var body = new { InvoiceAmount = amount, CurrencyIso = currency };
            var json = JsonConvert.SerializeObject(body);

            var client = CreateClient();
            var response = await client.PostAsync($"{_baseUrl}/v2/InitiatePayment",
                new StringContent(json, Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<MyFatoorahPaymentResponse> ExecutePayment(decimal amount, int methodId, string customerName, string callback, string errorUrl, string customerReference)
        {
            var body = new
            {
                InvoiceValue = amount,
                PaymentMethodId = methodId,
                CustomerName = customerName,
                CallBackUrl = callback,
                ErrorUrl = errorUrl,
                Language = "EN",
                CustomerReference = customerReference
            };

            var json = JsonConvert.SerializeObject(body);
            var client = CreateClient();

            var response = await client.PostAsync($"{_baseUrl}/v2/ExecutePayment",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MyFatoorahPaymentResponse>(content);
        }

        public async Task<MyFatoorahStatusResponse> GetPaymentStatus1(string paymentId)
        {
            var body = new { Key = paymentId, KeyType = "PaymentId" };
            var client = CreateClient();

            var json = JsonConvert.SerializeObject(body);
            var response = await client.PostAsync($"{_baseUrl}/v2/GetPaymentStatus",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MyFatoorahStatusResponse>(content);
        }


        //test version
        public async Task<MyFatoorahStatusResponse> GetPaymentStatus2(string paymentId)
        {
            // TEMP MOCK — Phase 3 only
            return new MyFatoorahStatusResponse
            {
                Data = new PaymentStatusData
                {
                    InvoiceStatus = "Paid",
                    CustomerReference = "35", 
                    InvoiceId = 12
                }
            };
        }

        public async Task<MyFatoorahStatusResponse> GetPaymentStatus(string paymentId)
        {
            // TEMP MOCK — Phase 4 subscription callback test
            return new MyFatoorahStatusResponse
            {
                Data = new PaymentStatusData
                {
                    InvoiceStatus = "Paid",
                    CustomerReference = "12",          
                    CustomerEmail = "ahmed@test.com", 
                    InvoiceId = 9999
                }
            };
        }







    }


}
