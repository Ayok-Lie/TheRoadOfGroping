using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.DomainService
{
    public interface IAnotherDomainService<TEntity, TPrimaryKey> : ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        //
        // 摘要:
        //     TEntity 仓储
        IAnotherBaseRepository<TEntity, TPrimaryKey> Repo { get; }

        //
        // 摘要:
        //     TEntity 查询器
        IQueryable<TEntity> Query { get; }

        //
        // 摘要:
        //     TEntity 查询器 - 不追踪
        IQueryable<TEntity> QueryAsNoTracking { get; }

        //
        // 摘要:
        //     获取 TEntity 并包含导航属性的查询器
        IQueryable<TEntity> GetIncludeQuery();

        //
        // 摘要:
        //     根据id查找 并包含导航属性
        //
        // 参数:
        //   id:
        Task<TEntity> FindByIdWithInclude(TPrimaryKey id);

        //
        // 摘要:
        //     获取一个 对象的新 Id
        //TPrimaryKey NewId();

        //
        // 摘要:
        //     根据id查找
        //
        // 参数:
        //   id:
        //     Id
        Task<TEntity> FindById(TPrimaryKey id);

        //
        // 摘要:
        //     检查 是否存在
        //
        // 参数:
        //   id:
        Task<TEntity> IsExist(TPrimaryKey id);

        //
        // 摘要:
        //     直接将 对象新增到数据库，经过校验方法
        //
        // 参数:
        //   :
        //     实例
        Task<TEntity> Create(TEntity entity);

        //
        // 摘要:
        //     直接将 对象更新到数据库，不经过校验方法
        //
        // 参数:
        //   :
        Task<TEntity> Update(TEntity entity);

        //
        // 摘要:
        //     根据id删除
        //
        // 参数:
        //   id:
        Task Delete(TPrimaryKey id);

        //
        // 摘要:
        //     根据对象删除
        //
        // 参数:
        //   :
        Task Delete(TEntity entity);

        //
        // 摘要:
        //     判断是否可以删除
        //
        // 参数:
        //   :
        //     对象
        Task ValidateOnDelete(TEntity entity);

        //
        // 摘要:
        //     校验数据正确性 - 新增和修改
        //
        // 参数:
        //   :
        //     T 实例
        //
        //   Base:
        //     TBase 实例
        Task ValidateOnCreateOrUpdate(TEntity entity);
    }
}