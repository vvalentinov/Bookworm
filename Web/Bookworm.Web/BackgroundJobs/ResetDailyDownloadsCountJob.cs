namespace Bookworm.Web.BackgroundJobs
{
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Microsoft.EntityFrameworkCore;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ResetDailyDownloadsCountJob : IJob
    {
        private readonly ApplicationDbContext dbContext;

        public ResetDailyDownloadsCountJob(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dbCommand = "UPDATE AspNetUsers SET DailyDownloadsCount = 0";
            await this.dbContext.Database.ExecuteSqlRawAsync(dbCommand);
        }
    }
}
