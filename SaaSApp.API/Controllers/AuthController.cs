using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("auth")]
    //[Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // ======================
        // ADMIN LOGIN
        // ======================
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(AdminLoginRequest req)
        {
            var result = await _auth.AdminLoginAsync(req.Email, req.Password);
            if (result == null)
                return Unauthorized(new { message = "Invalid admin credentials" });

            return Ok(result);
        }

        // ======================
        // TENANT OWNER LOGIN
        // ======================
        [HttpPost("tenant/login")]
        public async Task<IActionResult> TenantLogin(TenantOwnerLoginRequest req)
        {
            var result = await _auth.TenantOwnerLoginAsync(req.Email, req.Password);
            if (result == null)
                return Unauthorized(new { message = "Invalid tenant owner credentials" });

            return Ok(result);
        }

        // ======================
        // END USER LOGIN
        // ======================
        [HttpPost("user/login")]
        public async Task<IActionResult> EndUserLogin(EndUserLoginRequest req)
        {
            var result = await _auth.EndUserLoginAsync(req.Email, req.Password, req.TenantId);
            if (result == null)
                return Unauthorized(new { message = "Invalid user credentials" });

            return Ok(result);
        }

        // ======================
        // REGISTER TENANT OWNER
        // ======================
        [HttpPost("tenant/register")]
        public async Task<IActionResult> RegisterTenantOwner(TenantOwnerRegisterRequest req)
        {
            var result = await _auth.RegisterTenantOwnerAsync(req);
            return Ok(result);
        }

        // ======================
        // REGISTER END USER
        // ======================
        [HttpPost("user/register")]
        public async Task<IActionResult> RegisterEndUser(EndUserRegisterRequest req)
        {
            var result = await _auth.RegisterEndUserAsync(req);
            return Ok(result);
        }
    }
}
