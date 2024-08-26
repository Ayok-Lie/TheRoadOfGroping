using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.EntityFramework.Domain;
using RoadOfGroping.EntityFramework.UnitOfWorks;

namespace RoadOfGroping.EntityFramework
{
    public class RoadOfGropingDbContext : DbContext, IUnitOfWork, ITransaction
    {
        public virtual DbSet<Order> Order { get; set; }

        public RoadOfGropingDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系

            //modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            #endregion 注册领域模型与数据库的映射关系

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        #region IUnitOfWork

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion IUnitOfWork

        #region ITransaction

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = Database.BeginTransaction();
            return Task.FromResult(_currentTransaction);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion ITransaction
    }
}