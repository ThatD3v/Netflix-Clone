using NetflixClone.DTOs;

namespace NetflixClone.Services
{
    public interface IAdminService
    {
        Task<AdminResponse> RegisterAdminAsync(AdminRegisterRequest request);
        Task<AdminResponse> LoginAdminAsync(AdminLoginRequest request);
        Task<bool> IsAdminAsync(string userId);
        Task<bool> VerifyAdminSecretAsync(string secret);
    }
}