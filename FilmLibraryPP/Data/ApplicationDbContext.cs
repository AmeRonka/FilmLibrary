using FilmLibraryPP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Data
{
    /// <summary>
    /// Główny kontekst EF Core aplikacji. Dziedziczy po <see cref="IdentityDbContext{ApplicationUser}"/>,
    /// więc zarządza także tabelami ASP.NET Core Identity (AspNetUsers, AspNetRoles, itd.).
    /// Konfiguracja relacji domeny (Film ↔ Owner, Film ↔ Tag M2M) w <see cref="OnModelCreating"/>.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>Konstruktor wstrzykiwany przez DI.</summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>Wszystkie filmy w bazie.</summary>
        public DbSet<Film> Films => Set<Film>();

        /// <summary>Wszystkie tagi w bazie.</summary>
        public DbSet<Tag> Tags => Set<Tag>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Film>()
                .HasOne(f => f.Owner)
                .WithMany()
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Film>()
                .HasIndex(f => f.OwnerId);

            builder.Entity<Tag>()
                .HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tag>()
                .HasIndex(t => new { t.OwnerId, t.Name })
                .IsUnique();

            builder.Entity<Film>()
                .HasMany(f => f.Tags)
                .WithMany(t => t.Films)
                .UsingEntity(j => j.ToTable("FilmTags"));
        }
    }
}
