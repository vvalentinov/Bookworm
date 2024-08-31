namespace Bookworm.Web.Extensions
{
    using System.Reflection;

    using Bookworm.Data;
    using Bookworm.Data.Seeding;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Hubs;
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class WebApplicationExtensions
    {
        public static WebApplication Configure(this WebApplication app)
        {
            app
                .RegisterMappings()
                .SeedDatabase()
                .ConfigurePipeline()
                .MapRoutes()
                .MapHubs();

            return app;
        }

        private static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/HandleError").UseHsts();
            }

            app
                .UseStatusCodePagesWithReExecute("/Error/StatusCodeError/{0}")
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            return app;
        }

        private static WebApplication MapRoutes(this WebApplication app)
        {
            app.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            return app;
        }

        private static WebApplication RegisterMappings(this WebApplication app)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            return app;
        }

        private static WebApplication SeedDatabase(this WebApplication app)
        {
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

            return app;
        }

        private static WebApplication MapHubs(this WebApplication app)
        {
            app.MapHub<NotificationHub>("/hubs/notification");

            return app;
        }
    }
}
