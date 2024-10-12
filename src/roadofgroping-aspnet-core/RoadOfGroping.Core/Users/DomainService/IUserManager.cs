using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users.DomainService
{
    public interface IUserManager : IBasicDomainService<Entity.Users, string>
    {
    }
}