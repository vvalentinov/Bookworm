namespace Bookworm.Services.Data.Tests.Shared
{
    using Xunit;

    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
