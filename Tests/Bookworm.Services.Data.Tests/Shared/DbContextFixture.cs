namespace Bookworm.Services.Data.Tests.Shared
{
    using System;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class DbContextFixture : IDisposable
    {
        public DbContextFixture()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            this.DbContext = new ApplicationDbContext(dbContextOptionsBuilder);
            this.DbContext.Quotes.AddRange(GetQuotes());
            this.DbContext.QuotesLikes.AddRange(GetQuoteLikes());
            this.DbContext.Users.AddRange(GetUsers());
            this.DbContext.Roles.AddRange(GetRoles());
            this.DbContext.UserRoles.AddRange(GetUserRoles());
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

        private static IdentityUserRole<string>[] GetUserRoles()
        {
            var usersRoles = new IdentityUserRole<string>[3]
            {
                new () { UserId = "0fc3ea28-3165-440e-947e-670c90562320", RoleId = "fcea6658-70d4-4dac-a582-bb836b493323" },
                new () { UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7", RoleId = "b302b17d-1464-4346-9b53-3e64b76e46b0" },
                new () { UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114", RoleId = "b302b17d-1464-4346-9b53-3e64b76e46b0" },
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
            var users = new ApplicationUser[3]
            {
                new ()
                {
                    Id = "0fc3ea28-3165-440e-947e-670c90562320",
                    UserName = "Valentin",
                    NormalizedUserName = "VALENTIN",
                    Points = 4,
                },
                new ()
                {
                    Id = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    UserName = "Mike",
                    NormalizedUserName = "MIKE",
                    Points = 2,
                },
                new ()
                {
                    Id = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    UserName = "John",
                    NormalizedUserName = "JOHN",
                    Points = 4,
                },
            };

            return users;
        }

        private static QuoteLike[] GetQuoteLikes()
        {
            var quoteLikes = new QuoteLike[1]
            {
                new ()
                {
                    QuoteId = 7,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                },
            };

            return quoteLikes;
        }

        private static Quote[] GetQuotes()
        {
            var quotes = new Quote[10]
            {
                new ()
                {
                    Id = 1,
                    Content = "Knowledge is power",
                    AuthorName = "Sir Francis Bacon",
                    Type = QuoteType.GeneralQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = false,
                },
                new ()
                {
                    Id = 2,
                    Content = "The way to get started is to quit talking and begin doing",
                    AuthorName = "Walt Disney",
                    Type = QuoteType.GeneralQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                },
                new ()
                {
                    Id = 3,
                    Content = "The future belongs to those who believe in the beauty of their dreams",
                    AuthorName = "Eleanor Roosevelt",
                    Type = QuoteType.GeneralQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = true,
                },
                new ()
                {
                    Id = 4,
                    Content = "You must be the change you wish to see in the world",
                    AuthorName = "Mahatma Gandhi",
                    Type = QuoteType.GeneralQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = false,
                },
                new ()
                {
                    Id = 5,
                    Content = "There's no place like home",
                    MovieTitle = "The Wizard of Oz",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = true,
                },
                new ()
                {
                    Id = 6,
                    Content = "Carpe diem. Seize the day, boys. Make your lives extraordinary",
                    MovieTitle = "Dead Poets Society",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsDeleted = true,
                },
                new ()
                {
                    Id = 7,
                    Content = "Elementary, my dear Watson",
                    MovieTitle = "The Adventures of Sherlock Holmes",
                    Type = QuoteType.MovieQuote,
                    UserId = "a84ea5dc-a89e-442f-8e53-c874675bb114",
                    IsApproved = true,
                    Likes = 1,
                },
                new ()
                {
                    Id = 8,
                    Content = "It's alive! It's alive!",
                    MovieTitle = "Frankenstein",
                    Type = QuoteType.MovieQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                    IsApproved = true,
                },
                new ()
                {
                    Id = 9,
                    Content = "I'll be back!",
                    MovieTitle = "The Terminator",
                    Type = QuoteType.MovieQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = false,
                },
                new ()
                {
                    Id = 10,
                    Content = "Here's looking at you, kid.",
                    MovieTitle = "Casablanca",
                    Type = QuoteType.MovieQuote,
                    UserId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    IsApproved = true,
                },
            };

            return quotes;
        }
    }
}
