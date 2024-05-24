namespace Bookworm.Services.Data.Tests.Shared
{
    using System;
    using System.Reflection;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.NotificationsMessagesConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.NotificationsMessagesConstants;

    public class DbContextFixture : IDisposable
    {
        public DbContextFixture()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            this.DbContext = new ApplicationDbContext(dbContextOptionsBuilder);
            this.DbContext.Quotes.AddRange(GetQuotes());
            this.DbContext.QuotesLikes.AddRange(GetQuoteLikes());
            this.DbContext.Users.AddRange(GetUsers());
            this.DbContext.Roles.AddRange(GetRoles());
            this.DbContext.UserRoles.AddRange(GetUserRoles());
            this.DbContext.Notifications.AddRange(GetNotifications());
            this.DbContext.SaveChanges();
        }

        public ApplicationDbContext DbContext { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DbContext.Dispose();
            }
        }

        private static Notification[] GetNotifications()
        {
            var notifications = new Notification[9]
            {
                new ()
                {
                    Id = 1,
                    Content = string.Format(ApprovedQuoteNotification, "The way to get started is to quit talking and begin doing", QuoteUploadPoints),
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 2,
                    Content = string.Format(ApprovedQuoteNotification, "The future belongs to those who believe in the beauty of their dreams", QuoteUploadPoints),
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 3,
                    Content = string.Format(UnapprovedQuoteNotification, "I'll be back!", QuoteUploadPoints),
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 4,
                    Content = string.Format(UnapprovedQuoteNotification, "Knowledge is power", QuoteUploadPoints),
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 5,
                    Content = string.Format(UnapprovedQuoteNotification, "You must be the change you wish to see in the world", QuoteUploadPoints),
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                },
                new ()
                {
                    Id = 6,
                    Content = string.Format(ApprovedQuoteNotification, "Here's looking at you, kid.", QuoteUploadPoints),
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsDeleted = false,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 7,
                    Content = string.Format(ApprovedQuoteNotification, "Some content here!", QuoteUploadPoints),
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsDeleted = false,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 8,
                    Content = string.Format(ApprovedQuoteNotification, "There's no place like home", QuoteUploadPoints),
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 9,
                    Content = string.Format(ApprovedQuoteNotification, "Carpe diem. Seize the day, boys. Make your lives extraordinary", QuoteUploadPoints),
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsDeleted = false,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                },
            };

            return notifications;
        }

        private static IdentityUserRole<string>[] GetUserRoles()
        {
            var usersRoles = new IdentityUserRole<string>[4]
            {
                new () { UserId = "0fc3ea28-3165-440e-947e-670c90562320", RoleId = "fcea6658-70d4-4dac-a582-bb836b493323" },
                new () { UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7", RoleId = "b302b17d-1464-4346-9b53-3e64b76e46b0" },
                new () { UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114", RoleId = "b302b17d-1464-4346-9b53-3e64b76e46b0" },
                new () { UserId = "b1a9a91f-f7b1-4459-9864-4a4fdd6077c5", RoleId = "b302b17d-1464-4346-9b53-3e64b76e46b0" },
            };

            return usersRoles;
        }

        private static ApplicationRole[] GetRoles()
        {
            var roles = new ApplicationRole[2]
            {
                new ()
                {
                    Id = "fcea6658-70d4-4dac-a582-bb836b493323",
                    Name = "Administrator",
                },
                new ()
                {
                    Id = "b302b17d-1464-4346-9b53-3e64b76e46b0",
                    Name = "User",
                },
            };

            return roles;
        }

        private static ApplicationUser[] GetUsers()
        {
            return
            [
                new ()
                {
                    Id = "0fc3ea28-3165-440e-947e-670c90562320",
                    UserName = "Valentin",
                    Points = 8,
                },
                new ()
                {
                    Id = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    UserName = "Mike",
                    Points = 4,
                },
                new ()
                {
                    Id = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    UserName = "John",
                    Points = 4,
                },
                new ()
                {
                    Id = "b1a9a91f-f7b1-4459-9864-4a4fdd6077c5",
                    UserName = "Tom",
                    Points = 2,
                },
            ];
        }

        private static QuoteLike[] GetQuoteLikes()
        {
            return
            [
                new ()
                {
                    QuoteId = 7,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                },
                new ()
                {
                    QuoteId = 7,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                },
                new ()
                {
                    QuoteId = 10,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                },
            ];
        }

        private static Quote[] GetQuotes()
        {
            return
            [
                new ()
                {
                    Id = 1,
                    Content = "Knowledge is power",
                    AuthorName = "Sir Francis Bacon",
                    Type = QuoteType.GeneralQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 2,
                    Content = "The way to get started is to quit talking and begin doing",
                    AuthorName = "Walt Disney",
                    Type = QuoteType.GeneralQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 3,
                    Content = "The future belongs to those who believe in the beauty of their dreams",
                    AuthorName = "Eleanor Roosevelt",
                    Type = QuoteType.GeneralQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 4,
                    Content = "You must be the change you wish to see in the world",
                    AuthorName = "Mahatma Gandhi",
                    Type = QuoteType.GeneralQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 5,
                    Content = "There's no place like home",
                    MovieTitle = "The Wizard of Oz",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 6,
                    Content = "Carpe diem. Seize the day, boys. Make your lives extraordinary",
                    MovieTitle = "Dead Poets Society",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsDeleted = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 7,
                    Content = "Elementary, my dear Watson",
                    MovieTitle = "The Adventures of Sherlock Holmes",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = true,
                    Likes = 2,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 8,
                    Content = "It's alive! It's alive!",
                    MovieTitle = "Frankenstein",
                    Type = QuoteType.MovieQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 9,
                    Content = "I'll be back!",
                    MovieTitle = "The Terminator",
                    Type = QuoteType.MovieQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 10,
                    Content = "Here's looking at you, kid.",
                    MovieTitle = "Casablanca",
                    Type = QuoteType.MovieQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = true,
                    Likes = 1,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 11,
                    Content = "We accept the love we think we deserve.",
                    BookTitle = "The Perks of Being a Wallflower",
                    AuthorName = "Stephen Chbosky",
                    Type = QuoteType.BookQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 12,
                    Content = "I took a deep breath and listened to the old brag of my heart: I am, I am, I am.",
                    BookTitle = "The Bell Jar",
                    AuthorName = "Sylvia Plath",
                    Type = QuoteType.BookQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 13,
                    Content = "Love is or it ain’t. Thin love ain’t love at all.",
                    BookTitle = "Beloved",
                    AuthorName = "Toni Morrison",
                    Type = QuoteType.BookQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsDeleted = true,
                    CreatedOn = DateTime.UtcNow,
                },
                new ()
                {
                    Id = 14,
                    Content = "It is nothing to die; it is dreadful not to live.",
                    BookTitle = "Les Misérables",
                    AuthorName = "Victor Hugo",
                    Type = QuoteType.BookQuote,
                    UserId = "b1a9a91f-f7b1-4459-9864-4a4fdd6077c5",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow,
                },
            ];
        }
    }
}
