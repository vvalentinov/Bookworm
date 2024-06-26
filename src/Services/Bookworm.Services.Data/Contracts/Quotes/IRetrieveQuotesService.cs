﻿namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;

    public interface IRetrieveQuotesService
    {
        Task<QuoteViewModel> GetByIdAsync(int quoteId);

        Task<QuoteListingViewModel> GetAllByCriteriaAsync(string userId, GetQuotesApiDto getQuotesApiDto);

        Task<QuoteListingViewModel> GetAllApprovedAsync(int? page = null, string userId = null);

        Task<QuoteListingViewModel> GetAllUnapprovedAsync();

        Task<QuoteListingViewModel> GetAllDeletedAsync();

        Task<QuoteViewModel> GetRandomAsync();

        Task<int> GetUnapprovedCountAsync();

        Task<QuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page);

        Task<UploadQuoteViewModel> GetQuoteForEditAsync(int id, string userId);
    }
}
