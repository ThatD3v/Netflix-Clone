using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetflixClone.Services
{
    public class JWTService
    {
        private readonly string _issuer, _audience, _key;
        public JWTService(IConfiguration config)
        {
            _key = config.GetValue<string>("JWT:Key");
            _issuer = config.GetValue<string>("JWT:Issuer");

         _audience = config.GetValue<string>("JWT:Audience");
        }
        public string GenerateToken(string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims (data inside the token)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            //new Claim(JwtRegisteredClaimNames.Email, email),
            //new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer:_issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // token validity
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
