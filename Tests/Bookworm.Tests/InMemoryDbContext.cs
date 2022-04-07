namespace Bookworm.Tests
{
    using Bookworm.Data;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    public class InMemoryDbContext
    {
        private readonly SqliteConnection connection;
        private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

        public InMemoryDbContext()
        {
            this.connection = new SqliteConnection("Filename=:memory:");
            this.connection.Open();

            this.dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(this.connection)
                .Options;

            using var context = new ApplicationDbContext(dbContextOptions);

            context.Database.EnsureCreated();
        }

        public ApplicationDbContext CreateContext() => new ApplicationDbContext(dbContextOptions);

        public void Dispose() => connection.Dispose();
    }
}
