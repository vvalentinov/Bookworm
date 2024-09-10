namespace Bookworm.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Models;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Repositories;
    using Microsoft.EntityFrameworkCore.Storage;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        private readonly Dictionary<Type, object> repositories;

        private IDbContextTransaction currentTransaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            this.context = context;

            this.repositories = [];
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);

            if (!this.repositories.TryGetValue(type, out object value))
            {
                var repository = new EfRepository<TEntity>(this.context);
                value = repository;
                this.repositories[type] = value;
            }

            return (IRepository<TEntity>)value;
        }

        public IDeletableEntityRepository<TEntity> GetDeletableEntityRepository<TEntity>()
            where TEntity : class, IDeletableEntity
        {
            var type = typeof(TEntity);

            if (!this.repositories.TryGetValue(type, out object value))
            {
                var repository = new EfDeletableEntityRepository<TEntity>(this.context);
                value = repository;
                this.repositories[type] = value;
            }

            return (IDeletableEntityRepository<TEntity>)value;
        }

        public async Task<int> SaveChangesAsync()
            => await this.context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            this.currentTransaction ??= await this.context.Database.BeginTransactionAsync();
            return this.currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (this.currentTransaction != null)
            {
                await this.currentTransaction.CommitAsync();
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (this.currentTransaction != null)
            {
                await this.currentTransaction.RollbackAsync();
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.context?.Dispose();
            }
        }
    }
}
