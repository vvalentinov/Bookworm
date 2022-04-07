namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Moq;
    using Xunit;

    public class LanguagesServiceTests
    {
        private readonly List<Language> languagesList;
        private readonly LanguagesService languagesService;

        public LanguagesServiceTests()
        {
            this.languagesList = new List<Language>()
            {
                new Language()
                {
                    Name = "Bulgarian",
                },
                new Language()
                {
                    Name = "French",
                },
                new Language()
                {
                    Name = "Spanish",
                },
            };

            Mock<IRepository<Language>> mockLanguagesRepo = new Mock<IRepository<Language>>();
            mockLanguagesRepo.Setup(x => x.AllAsNoTracking()).Returns(this.languagesList.AsQueryable());
            mockLanguagesRepo.Setup(x => x.AddAsync(It.IsAny<Language>()))
                .Callback((Language language) => this.languagesList.Add(language));

            this.languagesService = new LanguagesService(mockLanguagesRepo.Object);
        }

        [Fact]
        public void LanguagesCountShouldBeCorrect()
        {
            IEnumerable<SelectListItem> languages = this.languagesService.GetAllLanguages();

            Assert.Equal(3, languages.Count());
            Assert.IsType<List<SelectListItem>>(languages);
        }
    }
}
