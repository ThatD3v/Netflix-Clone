using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;
        public AdminService(
            UserManager<User> userManager,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<AdminResponse> RegisterAdminAsync(AdminRegisterRequest request)
        {
            try
            {
                var isValidSecret = await VerifyAdminSecretAsync(request.AdminSecret);
                if (!isValidSecret)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "Invalid admin registration key"
                    };
                }
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }
    
                User user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                await _userManager.AddToRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "User");
                var (accessToken, refreshToken) = _tokenService.CreateTokens(user);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = request.RememberMe
                    ? DateTime.UtcNow.AddDays(30)
                    : DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);
                _logger.LogInformation($"Admin user registered: {request.Email}");
                return new AdminResponse
                {
                    Success = true,
                    Message = "Admin account created successfully",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new AdminUserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber,
                        IsAdmin = true,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin");
                return new AdminResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }
        public async Task<AdminResponse> LoginAdminAsync(AdminLoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "User is not an administrator"
                    };
                }
                if (!user.IsActive)
                {
                    return new AdminResponse
                    {
                        Success = false,
                        Message = "Account is deactivated"
                    };
                }
                user.LastLoginAt = DateTime.UtcNow;
                var (accessToken, refreshToken) = _tokenService.CreateTokens(user);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = request.RememberMe
                    ? DateTime.UtcNow.AddDays(30)
                    : DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);
                _logger.LogInformation($"Admin logged in: {request.Email}");
                return new AdminResponse
                {
                    Success = true,
                    Message = "Admin login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new AdminUserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber,
                        IsAdmin = true,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in admin");
                return new AdminResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }
        public async Task<bool> IsAdminAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                return await _userManager.IsInRoleAsync(user, "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin status");
                return false;
            }
        }
        public async Task<bool> VerifyAdminSecretAsync(string secret)
        {
            var adminSecret = _configuration["AdminRegistration:SecretKey"];
            return await Task.FromResult(adminSecret == secret);
        }
    }
}