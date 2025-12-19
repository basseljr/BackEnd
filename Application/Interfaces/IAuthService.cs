using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
      
            Task<AuthResponse?> AdminLoginAsync(string email, string password);
            Task<AuthResponse?> TenantOwnerLoginAsync(string email, string password);
            Task<AuthResponse?> EndUserLoginAsync(string email, string password, int tenantId);

            Task<AuthResponse> RegisterTenantOwnerAsync(TenantOwnerRegisterRequest req);
            Task<AuthResponse> RegisterEndUserAsync(EndUserRegisterRequest req);
        
    }
}
