namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class QuotesService : IQuotesService
    {
        private readonly IRepository<Quote> quoteRepository;
        private readonly IRepository<QuoteLike> quoteLikesRepository;
        private readonly IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository;
        private readonly IConfiguration configuration;
        private readonly IEmailSender emailSender;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public QuotesService(
            IRepository<Quote> quoteRepository,
            IRepository<QuoteLike> quoteLikesRepository,
            IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository,
            IConfiguration configuration,
            IEmailSender emailSender,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
            this.usersQuotesLikesRepository = usersQuotesLikesRepository;
            this.configuration = configuration;
            this.emailSender = emailSender;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task AddGeneralQuoteAsync(
            string content,
            string authorName,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                AuthorName = authorName,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task AddMovieQuoteAsync(
            string content,
            string movieTitle,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                MovieTitle = movieTitle,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task AddBookQuoteAsync(
            string content,
            string bookTitle,
            string author,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                BookTitle = bookTitle,
                AuthorName = author,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task ApproveQuote(int id, string userId)
        {
            var user = this.userRepository.All().First(x => x.Id == userId);
            string email = user.Email;

            var quote = this.quoteRepository.All().First(x => x.Id == id);
            quote.IsApproved = true;
            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
            user.Points += 3;
            await this.userManager.UpdateAsync(user);

            await this.emailSender.SendEmailAsync(
                    "bookwormproject@abv.bg",
                    "Bookworm",
                    $"{email}",
                    "Approved Quote",
                    $"Congratulations! Your quote {quote.Content} has been approved by the administrator! You have earned yourself 3 extra points!");
        }

        public async Task DeleteQuoteAsync(int quoteId)
        {
            var quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsDeleted = true;
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.Content = content;
            quote.AuthorName = authorName;
            quote.BookTitle = bookTitle;
            quote.MovieTitle = movieTitle;

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task<QuoteListingViewModel> GetAllQuotes(string userId)
        {
            var quotes = this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved && x.IsDeleted == false)
                .Select(x => new QuoteViewModel()
                {
                    Id = x.Id,
                    BookTitle = x.BookTitle,
                    MovieTitle = x.MovieTitle,
                    Content = x.Content,
                    AuthorName = x.AuthorName,
                })
                .OrderByDescending(x => x.Id)
                .ToList();

            foreach (var quote in quotes)
            {
                quote.Likes = await this.GetQuoteLikesAsync(quote.Id);
                quote.HasBeenLiked = userId != null && await this.CheckIfQuoteHasBeenLiked(quote.Id, userId);
            }

            return new QuoteListingViewModel()
            {
                Quotes = quotes,
            };
        }

        public IEnumerable<T> GetAllUnapprovedQuotes<T>()
        {
            return this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false && x.IsDeleted == false)
              .OrderBy(x => x.CreatedOn)
              .To<T>()
              .ToList();
        }

        public QuoteViewModel GetQuoteById(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);

            return new QuoteViewModel()
            {
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                Id = quoteId,
                MovieTitle = quote.MovieTitle,
            };
        }

        public T GetRandomQuote<T>()
        {
            return this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved == true && x.IsDeleted == false)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefault();
        }

        public UserQuotesViewModel GetUserQuotes(string userId)
        {
            var quotes = this.quoteRepository.AllAsNoTracking()
                                       .Where(x => x.UserId == userId && x.IsDeleted == false)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .Select(x => new QuoteViewModel()
                                       {
                                           Id = x.Id,
                                           Content = x.Content,
                                           AuthorName = x.AuthorName,
                                           BookTitle = x.BookTitle,
                                           MovieTitle = x.MovieTitle,
                                           IsApproved = x.IsApproved,
                                       }).ToList();

            int approvedQuotesCount = quotes.Where(x => x.IsApproved).Count();
            int unapprovedQuotesCount = quotes.Where(x => x.IsApproved == false).Count();

            return new UserQuotesViewModel()
            {
                Quotes = quotes,
                ApprovedQuotesCount = approvedQuotesCount,
                UnapprovedQuotesCount = unapprovedQuotesCount,
            };
        }

        public List<QuoteViewModel> SearchQuote(string content, string userId, QuoteType? type)
        {
            var quotes = this.quoteRepository.AllAsNoTracking()
                .Where(x => x.Content.Contains(content) && (userId == null || x.UserId == userId))
                .OrderByDescending(x => x.CreatedOn)
                                      .Select(x => new QuoteViewModel()
                                      {
                                          Id = x.Id,
                                          Content = x.Content,
                                          AuthorName = x.AuthorName,
                                          BookTitle = x.BookTitle,
                                          MovieTitle = x.MovieTitle,
                                          IsApproved = x.IsApproved,
                                      }).ToList();

            switch (type)
            {
                case QuoteType.ApprovedQuote:
                    return quotes.Where(q => q.IsApproved).ToList();
                case QuoteType.UnapprovedQuote:
                    return quotes.Where(q => q.IsApproved == false).ToList();
                case QuoteType.MovieQuote:
                    return quotes.Where(q => q.MovieTitle != null).ToList();
                case QuoteType.BookQuote:
                    return quotes.Where(q => q.BookTitle != null).ToList();
                case QuoteType.GeneralQuote:
                    return quotes.Where(q => q.BookTitle == null && q.MovieTitle == null).ToList();
                default: return quotes;
            }
        }

        public List<QuoteViewModel> GetQuotesByType(string userId, QuoteType type)
        {
            var quotes = this.quoteRepository.AllAsNoTracking()
                                       .Where(x => userId == null || x.UserId == userId)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .Select(x => new QuoteViewModel()
                                       {
                                           Id = x.Id,
                                           Content = x.Content,
                                           AuthorName = x.AuthorName,
                                           BookTitle = x.BookTitle,
                                           MovieTitle = x.MovieTitle,
                                           IsApproved = x.IsApproved,
                                       }).ToList();
            switch (type)
            {
                case QuoteType.ApprovedQuote:
                    return quotes.Where(q => q.IsApproved).ToList();
                case QuoteType.UnapprovedQuote:
                    return quotes.Where(q => q.IsApproved == false).ToList();
                case QuoteType.MovieQuote:
                    return quotes.Where(q => q.MovieTitle != null).ToList();
                case QuoteType.BookQuote:
                    return quotes.Where(q => q.BookTitle != null).ToList();
                case QuoteType.GeneralQuote:
                    return quotes.Where(q => q.MovieTitle == null && q.BookTitle == null).ToList();
                default: return quotes;
            }
        }

        public async Task<bool> QuoteExists(string content)
        {
            return await this.quoteRepository.AllAsNoTracking().AnyAsync(x => x.Content.Contains(content));
        }

        public async Task<int> LikeQuoteAsync(int quoteId, string userId)
        {
            QuoteLike quote = await this.quoteLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            if (quote == null)
            {
                quote = new QuoteLike()
                {
                    QuoteId = quoteId,
                    Likes = 1,
                };

                await this.quoteLikesRepository.AddAsync(quote);
                await this.quoteLikesRepository.SaveChangesAsync();
            }
            else
            {
                quote.Likes++;
            }

            UserQuoteLike userQuoteLike = await this.usersQuotesLikesRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.UserId == userId);

            if (userQuoteLike == null)
            {
                userQuoteLike = new UserQuoteLike()
                {
                    UserId = userId,
                    QuoteId = quoteId,
                };

                await this.usersQuotesLikesRepository.AddAsync(userQuoteLike);
                await this.usersQuotesLikesRepository.SaveChangesAsync();
            }
            else
            {
                userQuoteLike.IsDeleted = false;
            }

            await this.quoteLikesRepository.SaveChangesAsync();

            return quote.Likes;
        }

        public async Task<int> DislikeQuoteAsync(int quoteId, string userId)
        {
            QuoteLike quoteLike = await this.quoteLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            UserQuoteLike userQuoteLike = await this.usersQuotesLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.UserId == userId);

            userQuoteLike.IsDeleted = true;

            if (quoteLike.Likes > 0)
            {
                quoteLike.Likes--;
            }

            await this.quoteLikesRepository.SaveChangesAsync();
            return quoteLike.Likes;
        }

        public string GetMovieQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:MovieQuotes");
        }

        public string GetBookQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:BookQuotes");
        }

        public string GetGeneralQuoteImageUrl()
        {
            return this.configuration.GetValue<string>("QuotesImages:GeneralQuotes");
        }

        private async Task<int> GetQuoteLikesAsync(int quoteId)
        {
            QuoteLike quote = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            return quote == null ? 0 : quote.Likes;
        }

        private async Task<bool> CheckIfQuoteHasBeenLiked(int quoteId, string userId)
        {
            UserQuoteLike quote = await this.usersQuotesLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.QuoteId == quoteId &&
                    x.UserId == userId &&
                    x.IsDeleted == false);

            return quote != null;
        }
    }
}
