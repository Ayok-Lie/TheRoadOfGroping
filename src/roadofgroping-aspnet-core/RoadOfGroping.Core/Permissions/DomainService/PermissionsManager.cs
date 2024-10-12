using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Permissions.DomainService
{
    public class PermissionsManager : AnotherDomainService<PermissionOriginal, Guid>, IPermissionsManager
    {
        public PermissionsManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 获取所有权限（区分租户id）
        /// </summary>
        /// <returns></returns>
        public IQueryable<PermissionOriginal> GetAll()
        {
            return QueryAsNoTracking;
        }

        public override IQueryable<PermissionOriginal> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override async Task ValidateOnCreateOrUpdate(PermissionOriginal entity)
        {
            await Task.CompletedTask;
        }

        public override Task ValidateOnDelete(PermissionOriginal entity)
        {
            throw new NotImplementedException();
        }
    }
}