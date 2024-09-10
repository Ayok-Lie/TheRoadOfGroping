using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel.Response;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Core.Users;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    public class UserAppService : ApplicationService
    {
        private readonly IUserManager userManager;
        private readonly IMinioService minioService;

        public UserAppService(IServiceProvider serviceProvider, IUserManager userManager, IMinioService minioService) : base(serviceProvider)
        {
            this.userManager = userManager;
            this.minioService = minioService;
        }

        public async Task<List<RoadOfGropingUsers>> GetUserPageList()
        {
            return await userManager.QueryAsNoTracking.ToListAsync();
        }

        public async Task<RoadOfGropingUsers> GetUserById(Guid id)
        {
            var user = await userManager.QueryAsNoTracking.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                var picUrl = await minioService.PresignedGetObject("default-host", user.Avatar);
                user.Avatar = picUrl.Substring(0, picUrl.IndexOf("?"));
                return user;
            }
            {
                throw new ArgumentException("User not found");
            }
        }

        public async Task<RoadOfGropingUsers> CreateOrUpdateUser([FromForm] UserDto user)
        {
            PutObjectResponse putObjectResponse = null;
            if (user.File != null)
            {
                long size = user.File.Length;
                string saveFileName = $"{Path.GetFileName(user.File.FileName)}";
                var input = new UploadObjectInput();
                input.BucketName = "default-host";
                input.ObjectName = saveFileName;
                input.ContentType = user.File.ContentType;
                input.Stream = user.File.OpenReadStream();
                putObjectResponse = await minioService.UploadObjectAsync(input);
            }
            if (user.Id == Guid.Empty)
            {
                var data = ObjectMapper.Map<RoadOfGropingUsers>(user);
                data.Avatar = putObjectResponse?.ObjectName;
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
                existingUser.Avatar = putObjectResponse?.ObjectName;
                return await userManager.UpdateAsync(existingUser);
            }
        }

        public async Task DeleteUser(Guid id)
        {
            await userManager.DeleteAsync(id);
        }
    }
}