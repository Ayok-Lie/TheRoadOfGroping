using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public interface IAuthTokenManager
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(string userId);

        Task CreateJwtTokenForSwaggerAuth(string userId);

        Task<AuthTokenDto> RefreshAuthTokenAsync(RefreshTokenInput token);
    }
}