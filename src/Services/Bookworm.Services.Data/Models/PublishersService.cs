namespace Bookworm.Services.Data.Models
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class PublishersService : IPublishersService
    {
        private readonly IRepository<Publisher> publisherRepository;

        public PublishersService(IRepository<Publisher> publisherRepository)
        {
            this.publisherRepository = publisherRepository;
        }

        public async Task<OperationResult<Publisher>> GetPublisherWithNameAsync(string name)
        {
            var publisher = await this.publisherRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);

            return OperationResult.Ok(publisher);
        }
    }
}
