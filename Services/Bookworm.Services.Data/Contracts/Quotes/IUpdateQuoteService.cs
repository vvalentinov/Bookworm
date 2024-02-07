﻿namespace Bookworm.Services.Data.Contracts.Quotes
{
    using System.Threading.Tasks;

    public interface IUpdateQuoteService
    {
        Task ApproveQuoteAsync(int quoteId, string userId);

        Task DeleteQuoteAsync(int quoteId);

        Task SelfQuoteDeleteAsync(int quoteId, string userId);

        Task UndeleteQuoteAsync(int quoteId);

        Task UnapproveQuoteAsync(int quoteId);

        Task EditGeneralQuoteAsync(
            int quoteId,
            string content,
            string authorName);
    }
}
