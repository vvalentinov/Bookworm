namespace Bookworm.Web.BackgroundJobs
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Quartz;

    [DisallowConcurrentExecution]
    public class MarkOldNotificationsAsDeletedJob : IJob
    {
        private readonly INotificationService notificationService;

        public MarkOldNotificationsAsDeletedJob(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task Execute(IJobExecutionContext context)
            => await this.notificationService.MarkOldNotificationsAsDeletedAsync();
    }
}
