using RoadOfGroping.Application.AccountService.Dtos;
using RoadOfGroping.Core.Users.Dtos;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.AccountService
{
    public interface IAccountAppService : IApplicationService
    {
        Task<UserLoginDto> Login(LoginDto dto);

        Task ChangePassword(ChangePasswordInput input);

        Task PasswordReset(PasswordResetInput input);
    }
}