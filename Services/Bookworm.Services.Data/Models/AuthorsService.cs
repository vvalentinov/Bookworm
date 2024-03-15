namespace Bookworm.Services.Data.Models
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class AuthorsService : IAuthorsService
    {
        private readonly IRepository<Author> authorRepository;

        public AuthorsService(IRepository<Author> authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        public async Task<Author> GetAuthorWithNameAsync(string name)
            => await this.authorRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(a => a.Name == name.Trim());
    }
}
