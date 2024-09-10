namespace Bookworm.Data.Common.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> All();

        IQueryable<TEntity> AllAsNoTracking();

        Task AddAsync(TEntity entity);

        void RemoveRange(List<TEntity> entities);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void Approve(TEntity entity);

        void Unapprove(TEntity entity);

        Task<int> SaveChangesAsync();
    }
}
