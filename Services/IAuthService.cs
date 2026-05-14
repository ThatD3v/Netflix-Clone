using NetflixClone.DTOs;

namespace NetflixClone.Services;
public interface IAuthService
{
    Task<CheckUserResponse> CheckUserExistsAsync(CheckUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<AuthResponse> LogoutAsync(string userId);
    Task<UserDto?> GetUserByIdAsync(string userId);
}