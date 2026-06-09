using FilmLibraryPP.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Film> Films => Set<Film>();
    }
}
