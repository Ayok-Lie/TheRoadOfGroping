using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Application.UserService.Dtos;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Users.DomainService;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;
using RoadOfGroping.Repository.Repository;
using Yitter.IdGenerator;

namespace RoadOfGroping.Application.UserService
{
    /// <summary>
    /// 用户应用服务
    /// </summary>
    public class UserAppService : ApplicationService
    {
        private readonly IUserManager userManager;
        private readonly IMinioService minioService;
        private readonly IFileInfoManager _fileInfoManager;
        private readonly IIdGenerator _idGenerator;
        private readonly IBaseRepository<UserRoles> _userRoleRepository;

        /// <summary>
        /// 用户应用服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="userManager"></param>
        /// <param name="minioService"></param>
        /// <param name="fileInfoManager"></param>
        public UserAppService(IServiceProvider serviceProvider, IUserManager userManager, IMinioService minioService, IFileInfoManager fileInfoManager, IIdGenerator idGenerator, IBaseRepository<UserRoles> userRoleRepository) : base(serviceProvider)
        {
            this.userManager = userManager;
            this.minioService = minioService;
            _fileInfoManager = fileInfoManager;
            _idGenerator = idGenerator;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Users>> GetUserPageList()
        {
            return await userManager.QueryAsNoTracking.ToListAsync();
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Users> GetUserById(string id)
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
        public async Task<Users> CreateOrUpdateUser([FromForm] UserDto user)
        {
            string picUrl = String.Empty;
            if (user.File != null)
            {
                picUrl = await _fileInfoManager.UploadFileAsync(user.File, user.File.FileName);
            }
            if (user.Id == string.Empty)
            {
                var data = ObjectMapper.Map<Users>(user);
                data.Avater = picUrl;
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
                existingUser.Avater = picUrl;
                return await userManager.UpdateAsync(existingUser);
            }
        }

        public async Task CreateOrUpdate(CreateOrUpdateUserInput input)
        {
            if (input.Id == null || input.Id.IsNullOrEmpty())
            {
                await CreateUser(input);
            }
            else
            {
                await UpdateUser(input);
            }
        }

        /// <summary>
        /// 添加系统用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisplayName("添加系统用户")]
        public async Task CreateUser(CreateOrUpdateUserInput dto)
        {
            var user = ObjectMapper.Map<Users>(dto);
            user.Id = _idGenerator.NextId();
            string encode = _idGenerator.Encode(user.Id);
            user.Password = MD5Encryption.Encrypt(encode + dto.Password);
            var roles = dto.Roles?.Select(x => new UserRoles()
            {
                RoleId = x,
                UserId = user.Id
            }).ToList();
            await userManager.CreateAsync(user);
            await _userRoleRepository.InsertManyAsync(roles);
        }

        /// <summary>
        /// 更新系统用户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisplayName("更新系统用户信息")]
        public async Task UpdateUser(CreateOrUpdateUserInput dto)
        {
            var user = await userManager.FindByIdAsync(dto.Id);
            if (user == null) throw new Exception("无效参数");

            ObjectMapper.Map(dto, user);
            var roles = dto.Roles?.Select(x => new UserRoles()
            {
                RoleId = x,
                UserId = user.Id
            }).ToList();
            await userManager.UpdateAsync(user);
            await _userRoleRepository.DeleteAsync(x => x.UserId == user.Id);
            if (roles != null && roles.Any())
            {
                await _userRoleRepository.InsertManyAsync(roles);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUser(string id)
        {
            await userManager.DeleteAsync(id);
        }
    }
}