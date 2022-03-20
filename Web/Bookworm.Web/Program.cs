using System.Reflection;

using Azure.Storage.Blobs;
using Bookworm.Data;
using Bookworm.Data.Common;
using Bookworm.Data.Common.Repositories;
using Bookworm.Data.Models;
using Bookworm.Data.Repositories;
using Bookworm.Data.Seeding;
using Bookworm.Services.Data;
using Bookworm.Services.Data.Contracts;
using Bookworm.Services.Data.Models;
using Bookworm.Services.Mapping;
using Bookworm.Services.Messaging;
using Bookworm.Web.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }).AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetValue<string>("ConnectionStrings:StorageConnection")));

builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<IDbQueryRunner, DbQueryRunner>();

builder.Services.AddTransient<IEmailSender, NullMessageSender>();

builder.Services.AddTransient<ISettingsService, SettingsService>();

builder.Services.AddTransient<IQuotesService, QuotesService>();

builder.Services.AddTransient<ICategoriesService, CategoriesService>();

builder.Services.AddTransient<IBooksService, BooksService>();

builder.Services.AddTransient<IUploadBookService, UploadBookService>();

builder.Services.AddTransient<ILanguagesService, LanguagesService>();

builder.Services.AddTransient<IBlobService, BlobService>();

WebApplication app = builder.Build();

AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

using (IServiceScope serviceScope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });

app.Run();
