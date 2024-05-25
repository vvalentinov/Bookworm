namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.EntityFrameworkCore;

    public class AuthorsService : IAuthorsService
    {
        private readonly IRepository<Author> authorRepository;

        public AuthorsService(IRepository<Author> authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        public bool HasDuplicates(ICollection<UploadAuthorViewModel> authors)
            => authors
                .Select(x => x.Name.Trim())
                .GroupBy(author => author)
                .Any(group => group.Count() > 1);

        public async Task<Author> GetAuthorWithNameAsync(string name)
            => await this.authorRepository.AllAsNoTracking().FirstOrDefaultAsync(a => a.Name == name);
    }
}
