namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Identity;

    public class QuotesService : IQuotesService
    {
        private readonly IRepository<Quote> quoteRepository;
        private readonly IEmailSender emailSender;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public QuotesService(
            IRepository<Quote> quoteRepository,
            IEmailSender emailSender,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.quoteRepository = quoteRepository;
            this.emailSender = emailSender;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task AddQuoteAsync(
            string content,
            string authorName,
            string bookTitle,
            string movieTitle,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                AuthorName = authorName,
                BookTitle = bookTitle,
                MovieTitle = movieTitle,
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
            this.quoteRepository.Delete(quote);
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

        public IEnumerable<T> GetAllQuotes<T>()
        {
            return this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == true)
              .OrderBy(x => x.CreatedOn)
              .To<T>()
              .ToList();
        }

        public IEnumerable<T> GetAllUnapprovedQuotes<T>()
        {
            return this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false)
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
                        .Where(x => x.IsApproved == true)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefault();
        }

        public UserQuotesViewModel GetUserQuotes(string userId)
        {
            var quotes = this.quoteRepository.AllAsNoTracking()
                                       .Where(x => x.UserId == userId)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .Select(x => new QuoteViewModel()
                                       {
                                           Id = x.Id,
                                           AuthorName = x.AuthorName,
                                           BookTitle = x.BookTitle,
                                           MovieTitle = x.MovieTitle,
                                           Content = x.Content,
                                           IsApproved = x.IsApproved,
                                       }).ToList();

            int approvedQuotesCount = quotes.Where(x => x.IsApproved).Count();
            int unapprovedQuotesCount = quotes.Where(x => x.IsApproved == false).Count();

            return new UserQuotesViewModel() { Quotes = quotes, ApprovedQuotesCount = approvedQuotesCount, UnapprovedQuotesCount = unapprovedQuotesCount };
        }
    }
}
