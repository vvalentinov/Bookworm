namespace Bookworm.Web.ViewModels.Books
{
    using System;
    using System.Collections.Generic;

    public class BookListingViewModel
    {
        public string CategoryName { get; set; }

        public IEnumerable<BookViewModel> Books { get; set; }

        public int PageNumber { get; set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public int PreviousPageNumber => this.PageNumber - 1;

        public bool HasNextPage => this.PageNumber < this.PagesCount && this.BookCount > this.BooksPerPage;

        public int NextPageNumber => this.PageNumber + 1;

        public int PagesCount => (int)Math.Ceiling((double)this.BookCount / this.BooksPerPage);

        public int BookCount { get; set; }

        public int BooksPerPage { get; set; }
    }
}
