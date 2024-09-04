namespace Bookworm.Services.Data.Models
{
    using System.Threading.Tasks;

    using Bookworm.Common;
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

        public async Task<OperationResult<Author>> GetAuthorWithNameAsync(string name)
        {
            var author = await this.authorRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(a => a.Name == name);

            return OperationResult.Ok(author);
        }
    }
}
