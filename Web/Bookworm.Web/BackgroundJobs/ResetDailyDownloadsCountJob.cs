namespace Bookworm.Web.BackgroundJobs
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ResetDailyDownloadsCountJob : IJob
    {
        private readonly IUsersService usersService;

        public ResetDailyDownloadsCountJob(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public async Task Execute(IJobExecutionContext context)
            => await this.usersService.ResetDailyDownloadsCountAsync();
    }
}
