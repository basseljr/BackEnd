using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SaaSApp.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        // -------------------
        // ADMIN LOGIN
        // -------------------
        public async Task<AuthResponse?> AdminLoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == "Admin" && u.TenantId == 1);

            //if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            //    return null;

            return GenerateToken(user);
        }

        // -------------------
        // TENANT OWNER LOGIN
        // -------------------
        public async Task<AuthResponse?> TenantOwnerLoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == "Customer");

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        // -------------------
        // END USER LOGIN
        // -------------------
        public async Task<AuthResponse?> EndUserLoginAsync(string email, string password, int tenantId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == "EndUser" && u.TenantId == tenantId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        // -------------------
        // REGISTER TENANT OWNER
        // -------------------
        public async Task<AuthResponse> RegisterTenantOwnerAsync(TenantOwnerRegisterRequest req)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == req.Email);
            if (exists)
                throw new Exception("Email already exists");
            int previewTenantId = 5;

            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = "Customer",
                TenantId = previewTenantId,
                CreatedAt = DateTime.Now,
                TemplateId = req.TemplateId   


            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }


        // -------------------
        // REGISTER END USER
        // -------------------
        public async Task<AuthResponse> RegisterEndUserAsync(EndUserRegisterRequest req)
        {
            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = "EndUser",
                TenantId = req.TenantId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }

        // -------------------
        // TOKEN GENERATOR
        // -------------------
        private AuthResponse GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("email", user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("tenantId", user.TenantId?.ToString() ?? "0"),
                new Claim("fullName", user.FullName)
            };

            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpirationMinutes"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                Role = user.Role,
                TenantId = user.TenantId ?? 0,
                Email = user.Email,
                FullName = user.FullName,
                ExpiresAt = expiration
            };
        }
    }
}
