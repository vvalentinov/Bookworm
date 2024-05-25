namespace Bookworm.Web.Extensions
{
    using Bookworm.Common.Options;
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
    using Bookworm.Web.BackgroundJobs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;

    using static Bookworm.Data.IdentityOptionsProvider;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton(config);

            services.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IMailGunEmailSender, MailGunEmailSender>();

            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));

            // Quotes services
            services.AddScoped<IManageQuoteLikesService, ManageQuoteLikesService>();
            services.AddScoped<IRetrieveQuotesService, RetrieveQuotesService>();
            services.AddScoped<IUpdateQuoteService, UpdateQuoteService>();
            services.AddScoped<IUploadQuoteService, UploadQuoteService>();

            // Books services
            services.AddScoped<IValidateBookFilesSizesService, ValidateBookFilesSizesService>();
            services.AddScoped<IRetrieveBooksService, RetrieveBooksService>();
            services.AddScoped<IUploadBookService, UploadBookService>();
            services.AddScoped<IFavoriteBooksService, FavoriteBookService>();
            services.AddScoped<IUpdateBookService, UpdateBookService>();
            services.AddScoped<ISearchBooksService, SearchBooksService>();
            services.AddScoped<IDownloadBookService, DownloadBookService>();

            // Other services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ILanguagesService, LanguagesService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IRatingsService, RatingsService>();
            services.AddScoped<IVoteService, VotesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IPublishersService, PublishersService>();
            services.AddScoped<IAuthorsService, AuthorsService>();

            return services;
        }

        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = configuration["Authentication:Facebook:ClientId"];
                    facebookOptions.AppSecret = configuration["Authentication:Facebook:ClientSecret"];
                });

            return services;
        }

        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                string resetDailyDownloadsJobKey = nameof(ResetDailyDownloadsCountJob);
                string deleteOldNotificationsJobKey = nameof(MarkOldNotificationsAsDeletedJob);

                options
                    .AddJob<ResetDailyDownloadsCountJob>(JobKey.Create(resetDailyDownloadsJobKey))
                    .AddTrigger(triggerConfig => triggerConfig.ForJob(resetDailyDownloadsJobKey)
                    .WithCronSchedule("0 0 0 * * ?"));

                //options
                //    .AddJob<MarkOldNotificationsAsDeletedJob>(JobKey.Create(deleteOldNotificationsJobKey))
                //    .AddTrigger(triggerConfig => triggerConfig.ForJob(deleteOldNotificationsJobKey)
                //    .WithCronSchedule("0 */10 * ? * *"));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }

        public static IServiceCollection ConfigureOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MailGunEmailSenderOptions>(
                configuration.GetSection(MailGunEmailSenderOptions.MailGunEmailSender));

            return services;
        }

        public static IServiceCollection ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            return services;
        }

        public static IServiceCollection ConfigureApplicationCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Authentication/Login");
                options.LogoutPath = new PathString("/Account/Authentication/Logout");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.Cookie.HttpOnly = true;
            });

            return services;
        }

        public static IServiceCollection AddDistributedSqlServerCache(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = GetSqlServerConnection(config);
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });

            return services;
        }

        public static IServiceCollection AddApplicationDbContexts(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(GetSqlServerConnection(config)));

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(GetIdentityOptions)
                            .AddRoles<ApplicationRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddMvcControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            return services;
        }

        private static string GetSqlServerConnection(IConfiguration config)
            => config.GetConnectionString("SqlServerConnection");
    }
}
