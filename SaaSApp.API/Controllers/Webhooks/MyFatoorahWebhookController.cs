using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SaaSApp.API.Controllers.Webhooks
{
    [ApiController]
    [Route("api/webhooks/myfatoorah")]
    public class MyFatoorahWebhookController : ControllerBase
    {
        private readonly IMyFatoorahWebhookService _webhookService;

        public MyFatoorahWebhookController(IMyFatoorahWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            // Enable reading body multiple times
            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawJson = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            // Extract signature header (MyFatoorah uses one of these)
            var signature =
                Request.Headers["MyFatoorah-Signature"].FirstOrDefault()
                ?? Request.Headers["X-Signature"].FirstOrDefault()
                ?? Request.Headers["Signature"].FirstOrDefault();

            // Always return 200 (important for gateways)
            await _webhookService.ProcessAsync(rawJson, signature);

            return Ok();
        }
    }
}
