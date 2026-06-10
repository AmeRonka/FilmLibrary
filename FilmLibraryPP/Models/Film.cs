using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibraryPP.Models
{
    /// <summary>
    /// Podstawowa encja domeny — pojedynczy film w bibliotece użytkownika.
    /// Mapowana 1:1 na tabelę <c>Films</c> w bazie. Każdy film jest powiązany z dokładnie jednym właścicielem
    /// (<see cref="ApplicationUser"/>) oraz wieloma tagami (<see cref="Tag"/>) w relacji M2M.
    /// </summary>
    public class Film
    {
        /// <summary>Klucz główny, generowany przez bazę (IDENTITY).</summary>
        public int Id { get; set; }

        /// <summary>Tytuł filmu (wymagany, max 200 znaków).</summary>
        [Display(Name = "Tytuł")]
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(200, ErrorMessage = "Tytuł może mieć maksymalnie {1} znaków.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Rok produkcji (1888 — pierwszy znany film, do 2100 jako bufor).</summary>
        [Display(Name = "Rok")]
        [Range(1888, 2100, ErrorMessage = "Rok musi być z zakresu {1}–{2}.")]
        public int Year { get; set; }

        /// <summary>Gatunek filmu (opcjonalny, np. „Sci-Fi", „Dramat").</summary>
        [Display(Name = "Gatunek")]
        [StringLength(60)]
        public string? Genre { get; set; }

        /// <summary>Reżyser (opcjonalny).</summary>
        [Display(Name = "Reżyser")]
        [StringLength(120)]
        public string? Director { get; set; }

        /// <summary>Ocena 1-10, dziesiętna. Przechowywana jako <c>numeric</c> w Postgresie.</summary>
        [Display(Name = "Ocena")]
        [Range(1, 10, ErrorMessage = "Ocena musi być z zakresu {1}–{2}.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal? Rating { get; set; }

        /// <summary>Krótki opis fabuły.</summary>
        [Display(Name = "Opis")]
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>Prywatna recenzja właściciela.</summary>
        [Display(Name = "Recenzja")]
        [StringLength(4000)]
        public string? Review { get; set; }

        /// <summary>
        /// Data obejrzenia filmu. Kolumna typu <c>timestamp without time zone</c> w Postgresie —
        /// nie chcemy tutaj automatycznej konwersji stref czasowych z UTC.
        /// </summary>
        [Display(Name = "Data obejrzenia")]
        [DataType(DataType.Date)]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? WatchedDate { get; set; }

        /// <summary>Czy film został oznaczony jako obejrzany.</summary>
        [Display(Name = "Obejrzany")]
        public bool IsWatched { get; set; }

        /// <summary>URL plakatu (lokalny URL lub link do CDN TMDB).</summary>
        [Display(Name = "URL plakatu")]
        [Url(ErrorMessage = "Podaj poprawny adres URL.")]
        [StringLength(500)]
        public string? PosterUrl { get; set; }

        /// <summary>Tagi przypisane do filmu (relacja M2M, tabela <c>FilmTags</c>).</summary>
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// Identyfikator właściciela filmu (FK do <c>AspNetUsers.Id</c>).
        /// Null oznacza film bez właściciela — w praktyce nie powinno występować po seedach.
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>Nawigacja do właściciela. Ładowana eager-loadingiem przez admina.</summary>
        public ApplicationUser? Owner { get; set; }
    }
}
