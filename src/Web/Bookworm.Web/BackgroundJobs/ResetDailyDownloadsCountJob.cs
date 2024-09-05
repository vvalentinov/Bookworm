namespace Bookworm.Web.BackgroundJobs
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ResetDailyDownloadsCountJob : IJob
    {
        private readonly IDbQueryRunner queryRunner;

        public ResetDailyDownloadsCountJob(IDbQueryRunner queryRunner)
            => this.queryRunner = queryRunner;

        public async Task Execute(IJobExecutionContext context)
        {
            var dbCommand = "UPDATE AspNetUsers SET DailyDownloadsCount = 0";
            await this.queryRunner.RunQueryAsync(dbCommand);
        }
    }
}
