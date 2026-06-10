using FilmLibraryPP.Services.Tmdb;
using Xunit;

namespace FilmLibraryPP.Tests
{
    public class TmdbMapperTests
    {
        [Fact]
        public void ParseYear_ValidIsoDate_ReturnsYear()
        {
            var result = TmdbMapper.ParseYear("2024-03-15");
            Assert.Equal(2024, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-date")]
        [InlineData("2024")]
        public void ParseYear_InvalidOrEmpty_ReturnsNull(string? input)
        {
            var result = TmdbMapper.ParseYear(input);
            Assert.Null(result);
        }

        [Fact]
        public void ComposePosterUrl_WithPath_ReturnsFullCdnUrl()
        {
            var result = TmdbMapper.ComposePosterUrl("/abc123.jpg");
            Assert.Equal("https://image.tmdb.org/t/p/w500/abc123.jpg", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ComposePosterUrl_EmptyPath_ReturnsNull(string? input)
        {
            var result = TmdbMapper.ComposePosterUrl(input);
            Assert.Null(result);
        }

        [Fact]
        public void ToFilm_FullData_MapsAllFields()
        {
            var dto = new TmdbMovieDetails
            {
                Id = 27205,
                Title = "Incepcja",
                OriginalTitle = "Inception",
                ReleaseDate = "2010-07-16",
                Overview = "Złodziej kradnący sekrety przez sny.",
                PosterPath = "/poster.jpg",
                VoteAverage = 8.8m,
                Genres = new List<TmdbGenre>
                {
                    new TmdbGenre { Id = 28, Name = "Sci-Fi" },
                    new TmdbGenre { Id = 12, Name = "Thriller" }
                },
                Credits = new TmdbCredits
                {
                    Crew = new List<TmdbCrewMember>
                    {
                        new TmdbCrewMember { Name = "Wally Pfister", Job = "Director of Photography" },
                        new TmdbCrewMember { Name = "Christopher Nolan", Job = "Director" }
                    }
                }
            };

            var film = TmdbMapper.ToFilm(dto);

            Assert.Equal("Incepcja", film.Title);
            Assert.Equal(2010, film.Year);
            Assert.Equal("Sci-Fi", film.Genre);
            Assert.Equal("Christopher Nolan", film.Director);
            Assert.Equal(8.8m, film.Rating);
            Assert.Equal("Złodziej kradnący sekrety przez sny.", film.Description);
            Assert.Equal("https://image.tmdb.org/t/p/w500/poster.jpg", film.PosterUrl);
            Assert.False(film.IsWatched);
            Assert.Empty(film.Tags);
        }

        [Fact]
        public void ToFilm_MissingDirector_LeavesDirectorNull()
        {
            var dto = new TmdbMovieDetails
            {
                Title = "Mystery Film",
                ReleaseDate = "2020-01-01",
                Credits = new TmdbCredits
                {
                    Crew = new List<TmdbCrewMember>
                    {
                        new TmdbCrewMember { Name = "Some Person", Job = "Producer" }
                    }
                }
            };

            var film = TmdbMapper.ToFilm(dto);

            Assert.Null(film.Director);
        }

        [Fact]
        public void ToFilm_NoTitle_FallsBackToOriginal()
        {
            var dto = new TmdbMovieDetails
            {
                Title = "",
                OriginalTitle = "Original Name",
                ReleaseDate = "2020-01-01"
            };

            var film = TmdbMapper.ToFilm(dto);

            Assert.Equal("Original Name", film.Title);
        }

        [Fact]
        public void ToFilm_ZeroVoteAverage_LeavesRatingNull()
        {
            var dto = new TmdbMovieDetails
            {
                Title = "Unrated Film",
                ReleaseDate = "2024-01-01",
                VoteAverage = 0m
            };

            var film = TmdbMapper.ToFilm(dto);

            Assert.Null(film.Rating);
        }
    }
}
