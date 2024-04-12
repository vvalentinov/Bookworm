namespace Bookworm.Web
{
    using System.Reflection;

    using Bookworm.Data;
    using Bookworm.Data.Seeding;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Extensions;
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            using (IServiceScope serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                new ApplicationDbContextSeeder()
                    .SeedAsync(dbContext, serviceScope.ServiceProvider)
                    .GetAwaiter()
                    .GetResult();
            }

            app.ConfigurePipeline();

            app.Run();
        }
    }
}
