using System.Text.Json.Serialization;

namespace FilmLibraryPP.Services.Tmdb
{
    public class TmdbSearchResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<TmdbSearchResult> Results { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }

    public class TmdbSearchResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public decimal VoteAverage { get; set; }
    }

    public class TmdbMovieDetails
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public decimal VoteAverage { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenre> Genres { get; set; } = new();

        [JsonPropertyName("credits")]
        public TmdbCredits? Credits { get; set; }
    }

    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TmdbCredits
    {
        [JsonPropertyName("crew")]
        public List<TmdbCrewMember> Crew { get; set; } = new();
    }

    public class TmdbCrewMember
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("job")]
        public string Job { get; set; } = string.Empty;
    }
}
