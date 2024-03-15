namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public interface IPublishersService
    {
        Task<Publisher> GetPublisherWithNameAsync(string name);
    }
}
