using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibraryPP.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Display(Name = "Tytuł")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Rok")]
        public int Year { get; set; }

        [Display(Name = "Gatunek")]
        public string? Genre { get; set; }

        [Display(Name = "Reżyser")]
        public string? Director { get; set; }

        [Display(Name = "Ocena")]
        [Range(0, 10)]
        public decimal? Rating { get; set; }

        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Display(Name = "Recenzja")]
        public string? Review { get; set; }

        [Display(Name = "Data obejrzenia")]
        [DataType(DataType.Date)]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? WatchedDate { get; set; }

        [Display(Name = "Obejrzany")]
        public bool IsWatched { get; set; }

        [Display(Name = "Plakat")]
        public string? PosterUrl { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
