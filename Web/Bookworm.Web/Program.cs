namespace Bookworm.Web
{
    using System.Reflection;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Seeding;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationDbContexts(builder.Configuration);

            builder.Services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                            .AddRoles<ApplicationRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.Services.AddControllersWithViews(options =>
                            {
                                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                            }).AddRazorRuntimeCompilation();

            builder.Services.AddRazorPages();

            builder.Services.AddSingleton(builder.Configuration);

            builder.Services.AddScoped(x => new BlobServiceClient(builder.Configuration["ConnectionStrings:StorageConnection"]));

            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            WebApplication app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            using (IServiceScope serviceScope = app.Services.CreateScope())
            {
                ApplicationDbContext dbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                // dbContext.Database.Migrate();
                new ApplicationDbContextSeeder(builder.Configuration)
                    .SeedAsync(dbContext, serviceScope.ServiceProvider)
                    .GetAwaiter()
                    .GetResult();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
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

            app.Run();
        }
    }
}
