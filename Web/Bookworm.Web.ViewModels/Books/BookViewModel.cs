﻿namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class BookViewModel : IMapFrom<Book>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public int Year { get; set; }

        public int DownloadsCount { get; set; }

        public int PagesCount { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public string FileUrl { get; set; }

        public string CategoryName { get; set; }

        public IEnumerable<string> Authors { get; set; }
    }
}