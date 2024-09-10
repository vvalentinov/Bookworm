namespace Bookworm.Data
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common;
    using Microsoft.EntityFrameworkCore;

    public class DbQueryRunner : IDbQueryRunner
    {
        public DbQueryRunner(ApplicationDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.Context = context;
        }

        public ApplicationDbContext Context { get; set; }

        public async Task RunQueryAsync(
            string query,
            params object[] parameters)
            => await this.Context
                .Database
                .ExecuteSqlRawAsync(query, parameters);

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
