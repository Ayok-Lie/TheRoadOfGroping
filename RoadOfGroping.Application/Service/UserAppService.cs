using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Users;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    /// <summary>
    /// 用户应用服务
    /// </summary>
    public class UserAppService : ApplicationService
    {
        private readonly IUserManager userManager;
        private readonly IMinioService minioService;
        private readonly IFileInfoManager _fileInfoManager;

        /// <summary>
        /// 用户应用服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="userManager"></param>
        /// <param name="minioService"></param>
        /// <param name="fileInfoManager"></param>
        public UserAppService(IServiceProvider serviceProvider, IUserManager userManager, IMinioService minioService, IFileInfoManager fileInfoManager) : base(serviceProvider)
        {
            this.userManager = userManager;
            this.minioService = minioService;
            _fileInfoManager = fileInfoManager;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoadOfGropingUsers>> GetUserPageList()
        {
            return await userManager.QueryAsNoTracking.ToListAsync();
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<RoadOfGropingUsers> GetUserById(Guid id)
        {
            var user = await userManager.QueryAsNoTracking.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                return user;
            }
            {
                throw new ArgumentException("User not found");
            }
        }

        /// <summary>
        /// 创建或更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<RoadOfGropingUsers> CreateOrUpdateUser([FromForm] UserDto user)
        {
            string picUrl = String.Empty;
            if (user.File != null)
            {
                picUrl = await _fileInfoManager.UploadFileAsync(user.File, user.File.FileName);
            }
            if (user.Id == Guid.Empty)
            {
                var data = ObjectMapper.Map<RoadOfGropingUsers>(user);
                data.Avatar = picUrl;
                return await userManager.CreateAsync(data);
            }
            else
            {
                var existingUser = await userManager.QueryAsNoTracking.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    throw new ArgumentException("User not found");
                }
                ObjectMapper.Map(existingUser, user);
                existingUser.Avatar = picUrl;
                return await userManager.UpdateAsync(existingUser);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUser(Guid id)
        {
            await userManager.DeleteAsync(id);
        }
    }
}