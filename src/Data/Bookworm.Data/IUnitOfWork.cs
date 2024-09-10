namespace Bookworm.Data
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Models;
    using Bookworm.Data.Common.Repositories;
    using Microsoft.EntityFrameworkCore.Storage;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        IDeletableEntityRepository<TEntity> GetDeletableEntityRepository<TEntity>()
            where TEntity : class, IDeletableEntity;

        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
