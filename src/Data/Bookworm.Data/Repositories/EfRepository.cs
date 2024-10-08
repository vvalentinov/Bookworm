﻿namespace Bookworm.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Models;
    using Bookworm.Data.Common.Repositories;

    using Microsoft.EntityFrameworkCore;

    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public EfRepository(ApplicationDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            this.Context = context;
            this.DbSet = this.Context.Set<TEntity>();
        }

        protected DbSet<TEntity> DbSet { get; set; }

        protected ApplicationDbContext Context { get; set; }

        public virtual IQueryable<TEntity> All()
            => this.DbSet;

        public virtual IQueryable<TEntity> AllAsNoTracking()
            => this.DbSet.AsNoTracking();

        public virtual Task AddAsync(TEntity entity)
            => this.DbSet.AddAsync(entity).AsTask();

        public virtual void RemoveRange(List<TEntity> entities)
            => this.DbSet.RemoveRange(entities);

        public virtual void Approve(TEntity entity)
        {
            if (entity is IApprovableEntity approvableEntity)
            {
                approvableEntity.IsApproved = true;
                this.Update(entity);
            }
        }

        public virtual void Unapprove(TEntity entity)
        {
            if (entity is IApprovableEntity approvableEntity)
            {
                approvableEntity.IsApproved = false;
                this.Update(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            var entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
            => this.DbSet.Remove(entity);

        public Task<int> SaveChangesAsync()
            => this.Context.SaveChangesAsync();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context?.Dispose();
            }
        }
    }
}
