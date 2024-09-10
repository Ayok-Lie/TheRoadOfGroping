using RoadOfGroping.EntityFramework.Domain;

namespace RoadOfGroping.EntityFramework.Repositorys
{
    public interface IRepository<TEntity> where TEntity : Entity1, IAggregateRoot
    {
        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        bool Remove(Entity1 entity);

        Task<bool> RemoveAsync(Entity1 entity);
    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : Entity1<TKey>, IAggregateRoot
    {
        bool Delete(TKey id);

        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        TEntity Get(TKey id);

        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    }
}