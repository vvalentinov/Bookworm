namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class RandomBookService : IRandomBookService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public RandomBookService(IRepository<Category> categoriesRepository, IDeletableEntityRepository<Book> bookRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.bookRepository = bookRepository;
        }

        public IEnumerable<BookViewModel> GenerateBooks(string category, int countBooks)
        {
            if (category == "Random")
            {
                category = this.categoriesRepository
                    .AllAsNoTracking()
                    .OrderBy(x => Guid.NewGuid())
                    .First()
                    .Name;
            }

            int categoryId = this.categoriesRepository
                                 .AllAsNoTracking()
                                 .First(x => x.Name == category)
                                 .Id;

            List<BookViewModel> books = this.bookRepository
                .AllAsNoTracking()
                .Where(x => x.CategoryId == categoryId)
                .OrderBy(x => Guid.NewGuid())
                .Take(countBooks)
                .Select(x => new BookViewModel()
                {
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                    Id = x.Id,
                }).ToList();

            return books;
        }

        public IEnumerable<SelectListItem> GetCategories()
        {
            List<SelectListItem> categories = this.categoriesRepository
                                                  .AllAsNoTracking()
                                                  .OrderBy(x => x.Name)
                                                  .Select(x => new SelectListItem()
                                                  {
                                                      Text = x.Name,
                                                      Value = x.Name,
                                                  }).ToList();
            SelectListItem selectListItem = new SelectListItem()
            {
                Text = "Random",
                Value = "Random",
            };
            categories.Add(selectListItem);
            return categories;
        }
    }
}
