using System.Globalization;
using FilmLibraryPP.Models;

namespace FilmLibraryPP.Services.Tmdb
{
    /// <summary>
    /// Anti-corruption layer między DTO z TMDB a naszą encją <see cref="Film"/>.
    /// Wszystkie konwersje są statyczne — bez stanu, łatwe do testowania.
    /// </summary>
    public static class TmdbMapper
    {
        /// <summary>Bazowy URL serwowania plakatów z TMDB CDN (szerokość 500px).</summary>
        public const string PosterBaseUrl = "https://image.tmdb.org/t/p/w500";

        /// <summary>
        /// Mapuje pełne dane TMDB na nową instancję <see cref="Film"/>. Nie ustawia <c>OwnerId</c>
        /// — to zadanie kontrolera, który zna kontekst zalogowanego użytkownika.
        /// </summary>
        public static Film ToFilm(TmdbMovieDetails dto)
        {
            return new Film
            {
                Title = !string.IsNullOrWhiteSpace(dto.Title) ? dto.Title : (dto.OriginalTitle ?? "(bez tytułu)"),
                Year = ParseYear(dto.ReleaseDate) ?? DateTime.Today.Year,
                Description = string.IsNullOrWhiteSpace(dto.Overview) ? null : dto.Overview,
                Genre = dto.Genres.FirstOrDefault()?.Name,
                Director = ExtractDirector(dto.Credits),
                Rating = dto.VoteAverage > 0 ? Math.Round(dto.VoteAverage, 1) : null,
                PosterUrl = ComposePosterUrl(dto.PosterPath),
                IsWatched = false
            };
        }

        /// <summary>Doklejnia bazowy URL CDN do ścieżki plakatu. Zwraca <c>null</c> jeśli ścieżka pusta.</summary>
        public static string? ComposePosterUrl(string? posterPath)
        {
            if (string.IsNullOrWhiteSpace(posterPath))
            {
                return null;
            }
            return PosterBaseUrl + posterPath;
        }

        /// <summary>
        /// Parsuje datę premiery w formacie ISO (<c>yyyy-MM-dd</c>) i wyciąga rok.
        /// Zwraca <c>null</c> dla pustych, błędnych lub niepełnych dat.
        /// </summary>
        public static int? ParseYear(string? releaseDate)
        {
            if (string.IsNullOrWhiteSpace(releaseDate))
            {
                return null;
            }
            if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt.Year;
            }
            return null;
        }

        private static string? ExtractDirector(TmdbCredits? credits)
        {
            if (credits?.Crew == null)
            {
                return null;
            }
            return credits.Crew
                .FirstOrDefault(c => string.Equals(c.Job, "Director", StringComparison.OrdinalIgnoreCase))
                ?.Name;
        }
    }
}
