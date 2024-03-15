namespace Bookworm.Services.Data.Models
{
    using System.Threading.Tasks;

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

        public async Task<Publisher> GetPublisherWithNameAsync(string name)
            => await this.publisherRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name.Trim());
    }
}
