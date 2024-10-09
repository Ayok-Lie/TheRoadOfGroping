using RoadOfGroping.Core.Users.Dtos;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users
{
    public interface IUserManager : IBasicDomainService<RoadOfGropingUsers, Guid>
    {
        Task<RoadOfGropingUsers> Login(LoginDto userInfo);
    }
}