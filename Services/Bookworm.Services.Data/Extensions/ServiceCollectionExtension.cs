﻿namespace Microsoft.Extensions.DependencyInjection
{
    using Bookworm.Data;
    using Bookworm.Data.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Messaging;
    using CloudinaryDotNet;
    using global::Azure.Storage.Blobs;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            Cloudinary cloudinary = new (config.GetValue<string>("Cloudinary:CloudinaryUrl"));

            services.AddSingleton(cloudinary);

            services.AddSingleton(x => new BlobServiceClient(config.GetConnectionString("StorageConnection")));

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(config["SendGrid:ApiKey"]));

            services.AddTransient<ISettingsService, SettingsService>();

            services.AddTransient<IQuotesService, QuotesService>();

            services.AddTransient<ICategoriesService, CategoriesService>();

            services.AddTransient<IBooksService, BooksService>();

            services.AddTransient<IUploadBookService, UploadBookService>();

            services.AddTransient<ILanguagesService, LanguagesService>();

            services.AddTransient<IFavoriteBooksService, FavoriteBookService>();

            services.AddTransient<ICommentsService, CommentsService>();

            services.AddTransient<IBlobService, BlobService>();

            services.AddTransient<IRatingsService, RatingsService>();

            services.AddTransient<IVoteService, VotesService>();

            services.AddTransient<IRandomBookService, RandomBookService>();

            services.AddTransient<IUsersService, UsersService>();

            services.AddTransient<ICloudinaryService, CloudinaryService>();

            services.AddTransient<IDeleteBookService, DeleteBookService>();

            services.AddTransient<IEditBookService, EditBookService>();

            services.AddTransient<IApproveBookService, ApproveBookService>();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            return services;
        }

        public static IServiceCollection AddApplicationDbContexts(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }
}
