namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Authors;

    public interface IAuthorsService
    {
        Task<Author> GetAuthorWithNameAsync(string name);

        bool HasDuplicates(ICollection<UploadAuthorViewModel> authors);
    }
}
