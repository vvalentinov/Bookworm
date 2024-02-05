namespace Bookworm.Web.ViewModels.Quotes.Contracts
{
    using System;

    using Bookworm.Data.Models.Enums;

    public interface IQuoteViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public bool IsApproved { get; set; }

        public int Likes { get; set; }

        public bool IsLikedByUser { get; set; }

        public bool IsUserQuoteCreator { get; set; }

        public string Content { get; set; }

        public string AuthorName { get; set; }

        public string BookTitle { get; set; }

        public string MovieTitle { get; set; }

        public DateTime CreatedOn { get; set; }

        public QuoteType Type { get; set; }
    }
}
