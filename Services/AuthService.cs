using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetflixClone.DTOs;
using NetflixClone.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace NetflixClone.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionService _subscriptionService;

        public AuthService(
            UserManager<User> userManager,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            ISubscriptionService subscriptionService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _subscriptionService = subscriptionService;
            _configuration = configuration;
        }
        private static bool IsPhoneNumberInput(string input)
        {
            if(string.IsNullOrEmpty(input)) return false;
            return Regex.IsMatch(input, @"^\+?[1-9]\d{1,14}$");
        }
        public async Task<CheckUserResponse> CheckUserExistsAsync(CheckUserRequest request)
        {
            User? user = null;

            if (IsPhoneNumberInput(request.Identifier))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == request.Identifier);

                if (user != null)
                {
                    return new CheckUserResponse
                    {
                        Exists = true,
                        PhoneNumber = user.PhoneNumber,
                        Message = "User Found. Please Enter Your Password."
                    };
                }
            }
            else
            {
                user = await _userManager.FindByEmailAsync(request.Identifier);

                if (user != null)
                {
                    return new CheckUserResponse
                    {
                        Exists = true,
                        Email = user.Email,
                        Message = "User Found. Please Enter Your Password."
                    };
                }
            }
            return new CheckUserResponse
            {
                Exists = false,
                Message = "No Account Found. Please Create An Account."
            };
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            User? user = null;

            user = await _userManager.FindByEmailAsync(request.Identifier);

            if (user == null)
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == request.Identifier);
            }
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email/phone or password."
                };
            }
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email/phone or password."
                };
            }
            if (!user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account is deactivated."
                };
            }
            user.LastLoginAt = DateTime.UtcNow;
            var (accessToken, refreshToken) = _tokenService.CreateTokens(user);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = request.RememberMe
                ? DateTime.UtcNow.AddDays(30)
                : DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var hasActiveSubscription = await _subscriptionService.HasActiveSubscriptionAsync(user.Id);
            var subscription = await _subscriptionService.GetUserSubscriptionAsync(user.Id);
            _logger.LogInformation($"User {request.Identifier} logged in. Has subscription: {hasActiveSubscription}");

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapToUserDto(user),
                HasSubscription = hasActiveSubscription,
                Subscription = subscription,
                NeedsSubscription = !hasActiveSubscription
            };
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var isEmail = !IsPhoneNumberInput(request.Identifier);
            if (isEmail)
            {
                if (await _userManager.FindByEmailAsync(request.Identifier) != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "An account with this email already exists. Please login."
                    };
                }
            }
            else
            {
                if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.Identifier))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "An account with this phone number already exists. Please login."
                    };
                }
            }
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Identifier,
                Email = isEmail ? request.Identifier : null,
                PhoneNumber = !isEmail ? request.Identifier : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var (accessToken, refreshToken) = _tokenService.CreateTokens(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = request.RememberMe
                ? DateTime.UtcNow.AddDays(30)
                : DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapToUserDto(user),
                HasSubscription = false,
                Subscription = null,
                NeedsSubscription = true
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid access token"
                };
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponse> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Logged out successfully"
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var secretKey = _configuration["JwtSettings:SecretKey"]?? _configuration["Jwt:Key"]?? "MySuperSecretFallbackKeyHereThatIsLongEnough";
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return null;
            }
        }
        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}