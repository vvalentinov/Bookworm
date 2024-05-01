namespace Bookworm.Web
{
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddIdentity()
                .AddMvcControllers()
                .ConfigureCookiePolicy()
                .ConfigureApplicationCookie()
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

            app.Run();
        }
    }
}
