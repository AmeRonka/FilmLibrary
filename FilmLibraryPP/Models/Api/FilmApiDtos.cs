using System.ComponentModel.DataAnnotations;

namespace FilmLibraryPP.Models.Api
{
    /// <summary>Reprezentacja filmu zwracana przez API.</summary>
    public class FilmReadDto
    {
        /// <summary>Identyfikator filmu.</summary>
        public int Id { get; set; }

        /// <summary>Tytuł filmu.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Rok produkcji.</summary>
        public int Year { get; set; }

        /// <summary>Gatunek (np. „Sci-Fi", „Dramat").</summary>
        public string? Genre { get; set; }

        /// <summary>Reżyser.</summary>
        public string? Director { get; set; }

        /// <summary>Ocena w skali 1-10.</summary>
        public decimal? Rating { get; set; }

        /// <summary>Krótki opis filmu.</summary>
        public string? Description { get; set; }

        /// <summary>Prywatna recenzja właściciela.</summary>
        public string? Review { get; set; }

        /// <summary>Data obejrzenia (jeśli oznaczony jako obejrzany).</summary>
        public DateTime? WatchedDate { get; set; }

        /// <summary>Czy film został obejrzany.</summary>
        public bool IsWatched { get; set; }

        /// <summary>URL plakatu.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>Lista tagów przypisanych do filmu.</summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>Email właściciela filmu (widoczne tylko dla admina).</summary>
        public string? OwnerEmail { get; set; }
    }

    /// <summary>Dane wymagane do utworzenia nowego filmu.</summary>
    public class FilmCreateDto
    {
        /// <summary>Tytuł filmu (wymagany, do 200 znaków).</summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Rok produkcji (1888-2100).</summary>
        [Range(1888, 2100)]
        public int Year { get; set; }

        /// <summary>Gatunek.</summary>
        [StringLength(60)]
        public string? Genre { get; set; }

        /// <summary>Reżyser.</summary>
        [StringLength(120)]
        public string? Director { get; set; }

        /// <summary>Ocena 1-10.</summary>
        [Range(1, 10)]
        public decimal? Rating { get; set; }

        /// <summary>Opis.</summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>Prywatna recenzja.</summary>
        [StringLength(4000)]
        public string? Review { get; set; }

        /// <summary>Data obejrzenia.</summary>
        public DateTime? WatchedDate { get; set; }

        /// <summary>Czy film został obejrzany.</summary>
        public bool IsWatched { get; set; }

        /// <summary>URL plakatu.</summary>
        [Url]
        [StringLength(500)]
        public string? PosterUrl { get; set; }

        /// <summary>Lista nazw tagów. Nieistniejące zostaną utworzone w bazie tagów użytkownika.</summary>
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>Dane do aktualizacji filmu. Identyczne jak dla utworzenia.</summary>
    public class FilmUpdateDto : FilmCreateDto
    {
    }

    /// <summary>Strona wyników listy filmów.</summary>
    public class FilmsPageDto
    {
        /// <summary>Numer aktualnej strony (od 1).</summary>
        public int Page { get; set; }

        /// <summary>Liczba elementów na stronę.</summary>
        public int PageSize { get; set; }

        /// <summary>Łączna liczba filmów spełniających warunki.</summary>
        public int TotalCount { get; set; }

        /// <summary>Łączna liczba stron.</summary>
        public int TotalPages { get; set; }

        /// <summary>Lista filmów na tej stronie.</summary>
        public List<FilmReadDto> Items { get; set; } = new();
    }
}
