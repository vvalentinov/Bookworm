namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Settings;

    public interface ISettingsService
    {
        int GetCount();

        Task<IEnumerable<SettingViewModel>> GetAllAsync();
    }
}
