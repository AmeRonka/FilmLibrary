using System.Net;
using System.Text.Json;

namespace FilmLibraryPP.Services.Tmdb
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _http;
        private readonly ILogger<TmdbService> _logger;

        public TmdbService(HttpClient http, ILogger<TmdbService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<IReadOnlyList<TmdbSearchResult>> SearchAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<TmdbSearchResult>();
            }

            var url = $"search/movie?query={Uri.EscapeDataString(query)}&language=pl-PL&include_adult=false";

            try
            {
                using var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("TMDB search returned {Status} for query '{Query}'", response.StatusCode, query);
                    return Array.Empty<TmdbSearchResult>();
                }

                await using var stream = await response.Content.ReadAsStreamAsync(ct);
                var parsed = await JsonSerializer.DeserializeAsync<TmdbSearchResponse>(stream, cancellationToken: ct);
                return parsed?.Results ?? new List<TmdbSearchResult>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "TMDB search HTTP error for query '{Query}'", query);
                return Array.Empty<TmdbSearchResult>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "TMDB search timed out for query '{Query}'", query);
                return Array.Empty<TmdbSearchResult>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "TMDB search JSON parse error for query '{Query}'", query);
                return Array.Empty<TmdbSearchResult>();
            }
        }

        public async Task<TmdbMovieDetails?> GetMovieAsync(int id, CancellationToken ct = default)
        {
            var url = $"movie/{id}?language=pl-PL&append_to_response=credits";

            try
            {
                using var response = await _http.GetAsync(url, ct);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("TMDB movie {Id} returned {Status}", id, response.StatusCode);
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(ct);
                return await JsonSerializer.DeserializeAsync<TmdbMovieDetails>(stream, cancellationToken: ct);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "TMDB movie {Id} HTTP error", id);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "TMDB movie {Id} timed out", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "TMDB movie {Id} JSON parse error", id);
                return null;
            }
        }
    }
}
