namespace Bookworm.Web.Extensions
{
    using Bookworm.Common.Options;
    using Bookworm.Common.Options.Authentication;
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
        public static IServiceCollection Configure(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddQuartz()
                .AddIdentity()
                .AddInMemoryCache()
                .AddMvcControllers()
                .ConfigureCookiePolicy()
                .ConfigureOptions(config)
                .AddAuthentication(config)
                .AddRealTimeCommunication()
                .ConfigureApplicationCookie()
                .AddApplicationServices(config)
                .AddApplicationDbContexts(config);

            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config);

            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IMailGunEmailSender, MailGunEmailSender>();

            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));

            // Quotes services
            services.AddScoped<IUploadQuoteService, UploadQuoteService>();
            services.AddScoped<IUpdateQuoteService, UpdateQuoteService>();
            services.AddScoped<IRetrieveQuotesService, RetrieveQuotesService>();
            services.AddScoped<IManageQuoteLikesService, ManageQuoteLikesService>();

            // Books services
            services.AddScoped<IUploadBookService, UploadBookService>();
            services.AddScoped<IUpdateBookService, UpdateBookService>();
            services.AddScoped<ISearchBooksService, SearchBooksService>();
            services.AddScoped<IDownloadBookService, DownloadBookService>();
            services.AddScoped<IValidateBookService, ValidateBookService>();
            services.AddScoped<IFavoriteBookService, FavoriteBookService>();
            services.AddScoped<IRetrieveBooksService, RetrieveBooksService>();

            // Other services
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IVoteService, VotesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IAuthorsService, AuthorsService>();
            services.AddScoped<IRatingsService, RatingsService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<ILanguagesService, LanguagesService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IPublishersService, PublishersService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authOptions = new AuthenticationOptions();
            configuration.GetSection(AuthenticationOptions.Authentication).Bind(authOptions);

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = authOptions.Google.ClientId;
                    googleOptions.ClientSecret = authOptions.Google.ClientSecret;
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = authOptions.Facebook.ClientId;
                    facebookOptions.AppSecret = authOptions.Facebook.ClientSecret;
                });

            return services;
        }

        private static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                string resetDailyDownloadsJobKey = nameof(ResetDailyDownloadsCountJob);
                string deleteOldNotificationsJobKey = nameof(MarkOldNotificationsAsDeletedJob);

                options
                    .AddJob<ResetDailyDownloadsCountJob>(JobKey.Create(resetDailyDownloadsJobKey))
                    .AddTrigger(triggerConfig => triggerConfig.ForJob(resetDailyDownloadsJobKey)
                    .WithCronSchedule("0 0 0 * * ?"));

                options
                    .AddJob<MarkOldNotificationsAsDeletedJob>(JobKey.Create(deleteOldNotificationsJobKey))
                    .AddTrigger(triggerConfig => triggerConfig.ForJob(deleteOldNotificationsJobKey)
                    .WithCronSchedule("0 */10 * ? * *"));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }

        private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var authConfigSection = configuration.GetSection(AuthenticationOptions.Authentication);
            var mailGunConfigSection = configuration.GetSection(MailGunEmailSenderOptions.MailGunEmailSender);
            var azureBlobStorageConfigSection = configuration.GetSection(AzureBlobStorageOptions.AzureBlobStorage);

            services.Configure<AuthenticationOptions>(authConfigSection);
            services.Configure<MailGunEmailSenderOptions>(mailGunConfigSection);
            services.Configure<AzureBlobStorageOptions>(azureBlobStorageConfigSection);

            return services;
        }

        private static IServiceCollection ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            return services;
        }

        private static IServiceCollection ConfigureApplicationCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.LoginPath = new PathString("/Account/Authentication/Login");
                options.LogoutPath = new PathString("/Account/Authentication/Logout");
            });

            return services;
        }

        private static IServiceCollection AddApplicationDbContexts(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = GetSqlServerConnection(config);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(GetIdentityOptions)
                            .AddRoles<ApplicationRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        private static IServiceCollection AddMvcControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            return services;
        }

        private static IServiceCollection AddRealTimeCommunication(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }

        private static IServiceCollection AddInMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();

            return services;
        }

        private static string GetSqlServerConnection(IConfiguration config) => config.GetValue<string>("SqlServerConnection");
    }
}
