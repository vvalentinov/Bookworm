namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Settings;
    using Microsoft.EntityFrameworkCore;

    public class SettingsService : ISettingsService
    {
        private readonly IDeletableEntityRepository<Setting> settingsRepository;

        public SettingsService(IDeletableEntityRepository<Setting> settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        public int GetCount()
        {
            return this.settingsRepository.AllAsNoTracking().Count();
        }

        public async Task<IEnumerable<SettingViewModel>> GetAllAsync()
        {
            return await this.settingsRepository
                .All()
                .Select(setting => new SettingViewModel
                {
                    Id = setting.Id,
                    Name = setting.Name,
                    Value = setting.Value,
                    NameAndValue = $"{setting.Name} = {setting.Value}",
                })
                .ToListAsync();
        }
    }
}
