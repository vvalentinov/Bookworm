﻿namespace Microsoft.Extensions.DependencyInjection
{
    using Bookworm.Data;
    using Bookworm.Data.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Books;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(config["SendGrid:ApiKey"]));

            services.AddTransient<ISettingsService, SettingsService>();

            // Quotes services
            services.AddTransient<IManageQuoteLikesService, ManageQuoteLikesService>();
            services.AddTransient<IRetrieveQuotesService, RetrieveQuotesService>();
            services.AddTransient<IUpdateQuoteService, UpdateQuoteService>();
            services.AddTransient<IUploadQuoteService, UploadQuoteService>();

            // Books services
            services.AddTransient<IValidateUploadedBookService, ValidateUploadedBookService>();
            services.AddTransient<IRetrieveBooksService, RetrieveBooksService>();
            services.AddTransient<IUploadBookService, UploadBookService>();
            services.AddTransient<IFavoriteBooksService, FavoriteBookService>();
            services.AddTransient<IUpdateBookService, UpdateBookService>();
            services.AddTransient<ISearchBooksService, SearchBooksService>();
            services.AddTransient<IDownloadBookService, DownloadBookService>();

            // Other services
            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<ILanguagesService, LanguagesService>();
            services.AddTransient<ICommentsService, CommentsService>();
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IRatingsService, RatingsService>();
            services.AddTransient<IVoteService, VotesService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IPublishersService, PublishersService>();
            services.AddTransient<IAuthorsService, AuthorsService>();

            services.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

            return services;
        }

        public static IServiceCollection AddApplicationDbContexts(
            this IServiceCollection services,
            IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }
}
