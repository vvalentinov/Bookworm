namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;

    public interface IAuthorsService
    {
        Task<OperationResult<Author>> GetAuthorWithNameAsync(string name);
    }
}
