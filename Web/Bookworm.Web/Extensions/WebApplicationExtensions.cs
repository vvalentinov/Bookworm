﻿namespace Bookworm.Web.Extensions
{
    using System.Reflection;

    using Bookworm.Data;
    using Bookworm.Data.Seeding;
    using Bookworm.Services.Mapping;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            using (IServiceScope serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                new ApplicationDbContextSeeder(app.Configuration)
                    .SeedAsync(dbContext, serviceScope.ServiceProvider)
                    .GetAwaiter()
                    .GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage().UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error").UseHsts();
            }

            app
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            app.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.MapHub<NotificationHub>("/hubs/notification");

            return app;
        }
    }
}
