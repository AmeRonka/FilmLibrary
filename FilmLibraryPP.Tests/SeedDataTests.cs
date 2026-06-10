using FilmLibraryPP.Data;
using Xunit;

namespace FilmLibraryPP.Tests
{
    public class SeedDataTests
    {
        [Fact]
        public void Initialize_OnEmptyDb_Inserts8FilmsAnd4Tags()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();
            const string adminId = "test-admin-id";

            SeedData.Initialize(db, adminId);

            Assert.Equal(8, db.Films.Count());
            Assert.Equal(4, db.Tags.Count());
        }

        [Fact]
        public void Initialize_OnEmptyDb_AllFilmsBelongToAdmin()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();
            const string adminId = "test-admin-id";

            SeedData.Initialize(db, adminId);

            Assert.All(db.Films, film => Assert.Equal(adminId, film.OwnerId));
            Assert.All(db.Tags, tag => Assert.Equal(adminId, tag.OwnerId));
        }

        [Fact]
        public void Initialize_OnSecondCall_DoesNotDuplicate()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();
            const string adminId = "test-admin-id";

            SeedData.Initialize(db, adminId);
            SeedData.Initialize(db, adminId);

            Assert.Equal(8, db.Films.Count());
        }
    }
}
