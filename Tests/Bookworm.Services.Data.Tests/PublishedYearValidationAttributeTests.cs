namespace Bookworm.Services.Data.Tests
{
    using System;

    using Bookworm.Web.Infrastructure.Attributes;
    using Xunit;

    public class PublishedYearValidationAttributeTests
    {
        [Fact]
        public void IsValidReturnsFalseForYearsAfterCurrentYear()
        {
            BookYearValidationAttribute attribute = new BookYearValidationAttribute(2000);

            int year = DateTime.UtcNow.Year + 1;

            bool isValid = attribute.IsValid(year);

            Assert.False(isValid);
        }

        [Fact]
        public void IsValidShouldReturnTrueForYearEqualToCurrentYear()
        {
            BookYearValidationAttribute attribute = new BookYearValidationAttribute(2000);

            int year = DateTime.UtcNow.Year;

            bool isValid = attribute.IsValid(year);

            Assert.True(isValid);
        }

        [Fact]
        public void IsValidShouldReturnTrueForYearBetweenMinYearAndCurrentYear()
        {
            BookYearValidationAttribute attribute = new BookYearValidationAttribute(2000);

            int year = DateTime.UtcNow.Year - 10;

            bool isValid = attribute.IsValid(year);

            Assert.True(isValid);
        }
    }
}
