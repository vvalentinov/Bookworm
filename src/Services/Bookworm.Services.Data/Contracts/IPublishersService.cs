namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;

    public interface IPublishersService
    {
        Task<OperationResult<Publisher>> GetPublisherWithNameAsync(string name);
    }
}
