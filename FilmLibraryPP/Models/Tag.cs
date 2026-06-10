using System.ComponentModel.DataAnnotations;

namespace FilmLibraryPP.Models
{
    /// <summary>
    /// Tag (etykieta) przypisywany do filmów. Każdy tag należy do konkretnego użytkownika —
    /// dwóch różnych użytkowników może mieć tag o tej samej nazwie (np. „Ulubione"),
    /// ale w obrębie jednego użytkownika nazwa musi być unikalna (unique index na <c>(OwnerId, Name)</c>).
    /// </summary>
    public class Tag
    {
        /// <summary>Klucz główny.</summary>
        public int Id { get; set; }

        /// <summary>Nazwa tagu (max 50 znaków).</summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>FK do właściciela tagu (<c>AspNetUsers.Id</c>).</summary>
        public string? OwnerId { get; set; }

        /// <summary>Nawigacja do właściciela.</summary>
        public ApplicationUser? Owner { get; set; }

        /// <summary>Filmy oznaczone tym tagiem (druga strona M2M).</summary>
        public ICollection<Film> Films { get; set; } = new List<Film>();
    }
}
