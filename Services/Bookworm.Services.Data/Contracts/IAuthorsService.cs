namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public interface IAuthorsService
    {
        Task<Author> GetAuthorWithNameAsync(string name);
    }
}
