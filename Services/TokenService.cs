using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NetflixClone.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NetflixClone.Services;

public class TokenService(IConfiguration configuration, UserManager<User> userManager) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly UserManager<User> _userManager = userManager;

    public string CreateAccessToken(User user)
    {
        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new Claim("PhoneNumber", user.PhoneNumber));
        }

        var secretKey = _configuration["JwtSettings:SecretKey"] ?? _configuration["Jwt:Key"] ?? "MySuperSecretFallbackKeyHereThatIsLongEnough";
        var issuer = _configuration["JwtSettings:Issuer"] ?? _configuration["Jwt:Issuer"] ?? "NetflixClone";
        var audience = _configuration["JwtSettings:Audience"] ?? _configuration["Jwt:Audience"] ?? "NetflixCloneUsers";
        var expMinutesStr = _configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? _configuration["Jwt:DurationInMinutes"];

        var expirationMinutes = double.TryParse(expMinutesStr, out var minutes) ? minutes : 15;

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public (string AccessToken, string RefreshToken) CreateTokens(User user)
    {
        return (CreateAccessToken(user), CreateRefreshToken());
    }
}