using FilmLibraryPP.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FilmLibraryPP.Tests
{
    public class TagsM2MTests
    {
        [Fact]
        public async Task FilmWithTags_PersistsM2MRelationship()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();
            const string ownerId = "user-1";

            var tag1 = new Tag { Name = "Ulubione", OwnerId = ownerId };
            var tag2 = new Tag { Name = "Klasyka", OwnerId = ownerId };
            db.Tags.AddRange(tag1, tag2);

            var film = new Film
            {
                Title = "Test",
                Year = 2020,
                OwnerId = ownerId,
                Tags = new List<Tag> { tag1, tag2 }
            };
            db.Films.Add(film);
            await db.SaveChangesAsync();

            var loaded = await db.Films.Include(f => f.Tags).FirstAsync();
            Assert.Equal(2, loaded.Tags.Count);
            Assert.Contains(loaded.Tags, t => t.Name == "Ulubione");
            Assert.Contains(loaded.Tags, t => t.Name == "Klasyka");
        }

        [Fact]
        public async Task SameTagOnTwoFilms_SharesSingleTagRow()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();
            const string ownerId = "user-1";

            var sharedTag = new Tag { Name = "Klasyka", OwnerId = ownerId };
            db.Tags.Add(sharedTag);

            var film1 = new Film
            {
                Title = "Film 1",
                Year = 2020,
                OwnerId = ownerId,
                Tags = new List<Tag> { sharedTag }
            };
            var film2 = new Film
            {
                Title = "Film 2",
                Year = 2021,
                OwnerId = ownerId,
                Tags = new List<Tag> { sharedTag }
            };
            db.Films.AddRange(film1, film2);
            await db.SaveChangesAsync();

            Assert.Equal(1, db.Tags.Count(t => t.Name == "Klasyka"));
            var loadedTag = await db.Tags.Include(t => t.Films).FirstAsync();
            Assert.Equal(2, loadedTag.Films.Count);
        }

        [Fact]
        public async Task DifferentOwners_CanHaveSameTagName()
        {
            using var db = InMemoryDbFactory.CreateUniqueContext();

            db.Tags.Add(new Tag { Name = "Ulubione", OwnerId = "user-a" });
            db.Tags.Add(new Tag { Name = "Ulubione", OwnerId = "user-b" });
            await db.SaveChangesAsync();

            Assert.Equal(2, await db.Tags.CountAsync(t => t.Name == "Ulubione"));
        }
    }
}
