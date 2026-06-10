using FilmLibraryPP.Data;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Tests
{
    internal static class InMemoryDbFactory
    {
        public static ApplicationDbContext CreateUniqueContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }
    }
}
