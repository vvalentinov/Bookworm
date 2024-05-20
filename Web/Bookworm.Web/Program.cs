namespace Bookworm.Web
{
    using Bookworm.Services.Messaging.Hubs;
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            builder.Services
                .AddQuartz()
                .AddIdentity()
                .AddMvcControllers()
                .ConfigureCookiePolicy()
                .ConfigureApplicationCookie()
                .ConfigureOptions(builder.Configuration)
                .AddAuthentication(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddApplicationDbContexts(builder.Configuration)
                .AddDistributedSqlServerCache(builder.Configuration);

            var app = builder.Build();

            app
                .RegisterMappings()
                .SeedDatabase()
                .ConfigurePipeline()
                .MapRoutes();

            app.MapHub<NotificationHub>("/hubs/notification");

            app.Run();
        }
    }
}
