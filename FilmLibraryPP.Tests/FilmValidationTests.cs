using System.ComponentModel.DataAnnotations;
using FilmLibraryPP.Models;
using Xunit;

namespace FilmLibraryPP.Tests
{
    public class FilmValidationTests
    {
        private static List<ValidationResult> Validate(Film film)
        {
            var ctx = new ValidationContext(film);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(film, ctx, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ValidFilm_PassesValidation()
        {
            var film = new Film
            {
                Title = "Incepcja",
                Year = 2010,
                Genre = "Sci-Fi",
                Director = "Christopher Nolan",
                Rating = 8.8m,
                PosterUrl = "https://example.com/poster.jpg"
            };

            var errors = Validate(film);

            Assert.Empty(errors);
        }

        [Fact]
        public void EmptyTitle_FailsRequiredValidation()
        {
            var film = new Film
            {
                Title = "",
                Year = 2010
            };

            var errors = Validate(film);

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Film.Title)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(-5)]
        public void RatingOutOfRange_FailsValidation(int rating)
        {
            var film = new Film
            {
                Title = "Test",
                Year = 2010,
                Rating = rating
            };

            var errors = Validate(film);

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Film.Rating)));
        }

        [Theory]
        [InlineData(1887)]
        [InlineData(2101)]
        public void YearOutOfRange_FailsValidation(int year)
        {
            var film = new Film
            {
                Title = "Test",
                Year = year
            };

            var errors = Validate(film);

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Film.Year)));
        }

        [Fact]
        public void TitleTooLong_FailsStringLengthValidation()
        {
            var film = new Film
            {
                Title = new string('x', 201),
                Year = 2010
            };

            var errors = Validate(film);

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Film.Title)));
        }

        [Fact]
        public void InvalidPosterUrl_FailsUrlValidation()
        {
            var film = new Film
            {
                Title = "Test",
                Year = 2010,
                PosterUrl = "not-a-url"
            };

            var errors = Validate(film);

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Film.PosterUrl)));
        }
    }
}
