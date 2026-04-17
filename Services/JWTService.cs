using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetflixClone.Services
{
    public class JWTService
    {
        private readonly string? _issuer, _audience, _key;
        public JWTService(IConfiguration config)
        {
            _key = config["JWT:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");
            _issuer = config["JWT:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is missing in appsettings.json");
            _audience = config["JWT:Audience"]
                ?? throw new InvalidOperationException("JWT Audience is missing in appsettings.json");
        }
        public string GenerateToken(string userId, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer:_issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
