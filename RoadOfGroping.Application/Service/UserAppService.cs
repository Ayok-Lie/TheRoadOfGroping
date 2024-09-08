using System.Net;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Core.Users;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    public class UserAppService : ApplicationService
    {
        private readonly IUserManager userManager;

        public UserAppService(IServiceProvider serviceProvider, IUserManager userManager) : base(serviceProvider)
        {
            this.userManager = userManager;
        }

        public async Task<List<RoadOfGropingUsers>> GetUserPageList()
        {
            return await userManager.QueryAsNoTracking.ToListAsync();
        }

        public async Task<RoadOfGropingUsers> GetUserById(Guid id)
        {
            return await userManager.QueryAsNoTracking.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<RoadOfGropingUsers> GetUserByUsername(string username)
        {
            return await userManager.QueryAsNoTracking.SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<RoadOfGropingUsers> CreateUser(UserDto user)
        {
            var data = ObjectMapper.Map<RoadOfGropingUsers>(user);
            return await userManager.CreateAsync(data);
        }

        public async Task<RoadOfGropingUsers> UpdateUser(UserDto user)
        {
            var existingUser = await userManager.QueryAsNoTracking.FirstOrDefaultAsync(u => u.Id == user.Id);
            throw new NotImplementedException();
            //return await userManager.UpdateAsync(user);
        }

        public async Task DeleteUser(Guid id)
        {
            await userManager.DeleteAsync(id);
        }
    }
}