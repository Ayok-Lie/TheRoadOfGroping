using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.UserAppService
{
    public class UserInfoAppService : ApplicationService
    {
        public string Delete(int id)
        {
            return $"You deleted user with id {id}";
        }
    }
}