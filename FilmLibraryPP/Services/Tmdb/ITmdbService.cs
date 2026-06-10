namespace FilmLibraryPP.Services.Tmdb
{
    /// <summary>
    /// Wrapper na API The Movie Database (TMDB). Daje dwie operacje wystarczające
    /// do importu filmów: wyszukiwanie po tytule oraz pobranie szczegółów filmu.
    /// </summary>
    public interface ITmdbService
    {
        /// <summary>
        /// Wyszukuje filmy po tytule. Zwraca pustą listę przy błędach sieci, timeoutach lub błędach deserializacji
        /// (logując problem) — nigdy nie rzuca wyjątkiem do warstwy MVC.
        /// </summary>
        /// <param name="query">Tekst wyszukiwania (tytuł, fragment, oryginalny tytuł).</param>
        /// <param name="ct">Token anulowania (przekazywany z kontekstu żądania).</param>
        Task<IReadOnlyList<TmdbSearchResult>> SearchAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Pobiera szczegóły filmu o podanym TMDB ID, wraz z listą crew (do wyciągnięcia reżysera).
        /// Zwraca <c>null</c> przy 404, błędach sieci lub deserializacji.
        /// </summary>
        /// <param name="id">Identyfikator filmu w bazie TMDB.</param>
        /// <param name="ct">Token anulowania.</param>
        Task<TmdbMovieDetails?> GetMovieAsync(int id, CancellationToken ct = default);
    }
}
