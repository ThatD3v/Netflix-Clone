using NetflixClone.Models;

namespace NetflixClone.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
    (string AccessToken, string RefreshToken) CreateTokens(User user);
}