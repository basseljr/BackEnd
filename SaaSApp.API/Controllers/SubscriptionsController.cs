using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscription;

    public SubscriptionsController(ISubscriptionService subscription)
    {
        _subscription = subscription;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartSubscription(StartSubscriptionRequest request)
    {
        var userName = User.Identity?.Name ?? "TenantUser";

        var paymentUrl = await _subscription.StartSubscriptionAsync(request, userName);

        return Ok(new { paymentUrl });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] StartSubscriptionRequest request)
    {
        var url = await _subscription.StartSubscriptionAsync(request, request.CustomerEmail);
        return Ok(new { paymentUrl = url });
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string paymentId)
    {
        if (string.IsNullOrWhiteSpace(paymentId))
            return Redirect("http://localhost:4200/subscribe/failed");

        var result = await _subscription.HandleCallbackAsync(paymentId);

        if (!result.IsSuccess)
            return Redirect("http://localhost:4200/subscribe/failed");

        // If you already have a subscribe success page:
        return Redirect($"http://localhost:4200/subscribe/success?tenant={result.TenantSubdomain}");
    }


    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _subscription.GetPlansAsync();
        return Ok(plans);
    }

}
