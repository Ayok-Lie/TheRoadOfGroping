using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public interface IAuthTokenService
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(UserAuthDto user);

        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token);
    }
}