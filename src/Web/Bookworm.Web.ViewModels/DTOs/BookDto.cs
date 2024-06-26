﻿namespace Bookworm.Web.ViewModels.DTOs
{
    using System.Collections.Generic;

    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Http;

    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public int PagesCount { get; set; }

        public int Year { get; set; }

        public IFormFile BookFile { get; set; }

        public IFormFile ImageFile { get; set; }

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }

        public IList<UploadAuthorViewModel> Authors { get; set; }
    }
}
