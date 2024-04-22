namespace Bookworm.Web.Extensions
{
    using Bookworm.Data;
    using Bookworm.Data.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));

            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IEmailSender, MailKitEmailSender>();

            // Quotes services
            services.AddScoped<IManageQuoteLikesService, ManageQuoteLikesService>();
            services.AddScoped<IRetrieveQuotesService, RetrieveQuotesService>();
            services.AddScoped<IUpdateQuoteService, UpdateQuoteService>();
            services.AddScoped<IUploadQuoteService, UploadQuoteService>();

            // Books services
            services.AddScoped<IValidateUploadedBookService, ValidateUploadedBookService>();
            services.AddScoped<IRetrieveBooksService, RetrieveBooksService>();
            services.AddScoped<IUploadBookService, UploadBookService>();
            services.AddScoped<IFavoriteBooksService, FavoriteBookService>();
            services.AddScoped<IUpdateBookService, UpdateBookService>();
            services.AddScoped<ISearchBooksService, SearchBooksService>();
            services.AddScoped<IDownloadBookService, DownloadBookService>();

            // Other services
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ILanguagesService, LanguagesService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IRatingsService, RatingsService>();
            services.AddScoped<IVoteService, VotesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IPublishersService, PublishersService>();
            services.AddScoped<IAuthorsService, AuthorsService>();

            services.AddSignalR();

            services.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

            services.AddApplicationDbContexts(config);

            services.AddRazorPages();

            services.AddControllersWithViews(opt => opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
                .AddRazorRuntimeCompilation();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton(config);

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                            .AddRoles<ApplicationRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = GetSqlServerConnection(config);
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Identity/User/Login");
            });

            return services;
        }

        private static IServiceCollection AddApplicationDbContexts(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(GetSqlServerConnection(config)));
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }

        private static string GetSqlServerConnection(IConfiguration config)
            => config.GetConnectionString("SqlServerConnection");
    }
}
