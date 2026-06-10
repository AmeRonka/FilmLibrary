using FilmLibraryPP.Models;

namespace FilmLibraryPP.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context, string? ownerId = null)
        {
            if (context.Films.Any())
            {
                BackfillOrphans(context, ownerId);
                return;
            }

            var tagNames = new[] { "Ulubione", "Klasyka", "Do obejrzenia w weekend", "Premiera" };
            var tags = tagNames.ToDictionary(
                name => name,
                name => new Tag { Name = name, OwnerId = ownerId });

            context.Tags.AddRange(tags.Values);

            var films = new List<Film>
            {
                new Film
                {
                    Title = "Incepcja",
                    Year = 2010,
                    Genre = "Sci-Fi",
                    Director = "Christopher Nolan",
                    Rating = 8.8m,
                    Description = "Złodziej, który kradnie sekrety korporacyjne za pomocą technologii dzielenia się snami, dostaje odwrotne zadanie: zaszczepić ideę w umyśle prezesa.",
                    Review = "Genialna konstrukcja, ogląda się z otwartymi ustami.",
                    WatchedDate = new DateTime(2026, 1, 25),
                    IsWatched = true,
                    PosterUrl = "https://placehold.co/300x450/1a1a2e/eaeaea?text=Incepcja",
                    Tags = new List<Tag> { tags["Ulubione"], tags["Klasyka"] }
                },
                new Film
                {
                    Title = "Skazani na Shawshank",
                    Year = 1994,
                    Genre = "Dramat",
                    Director = "Frank Darabont",
                    Rating = 9.3m,
                    Description = "Dwóch więźniów buduje przyjaźń przez wiele lat, znajdując pocieszenie i odkupienie poprzez akty zwykłej ludzkiej przyzwoitości.",
                    Review = "Najlepszy film o nadziei jaki widziałam.",
                    WatchedDate = new DateTime(2023, 8, 14),
                    IsWatched = true,
                    PosterUrl = "https://placehold.co/300x450/2d4a3e/eaeaea?text=Shawshank",
                    Tags = new List<Tag> { tags["Ulubione"], tags["Klasyka"] }
                },
                new Film
                {
                    Title = "Pulp Fiction",
                    Year = 1994,
                    Genre = "Kryminał",
                    Director = "Quentin Tarantino",
                    Rating = 8.9m,
                    Description = "Życie dwóch płatnych zabójców mafii, boksera, gangstera i jego żony oraz pary rabusiów restauracji splata się w czterech opowieściach o przemocy i odkupieniu.",
                    Review = "Dialogi to miód. Nie nudzi się nigdy.",
                    WatchedDate = new DateTime(2024, 7, 22),
                    IsWatched = true,
                    PosterUrl = "https://placehold.co/300x450/8b2635/eaeaea?text=Pulp+Fiction",
                    Tags = new List<Tag> { tags["Klasyka"] }
                },
                new Film
                {
                    Title = "Interstellar",
                    Year = 2014,
                    Genre = "Sci-Fi",
                    Director = "Christopher Nolan",
                    Rating = 8.7m,
                    Description = "Zespół eksploratorów przemierza tunel czasoprzestrzenny w kosmosie w próbie zapewnienia ludzkości przetrwania.",
                    Review = "Końcówka rozwala. Hans Zimmer jak zawsze.",
                    WatchedDate = new DateTime(2025, 3, 11),
                    IsWatched = true,
                    PosterUrl = "https://placehold.co/300x450/0d1b2a/eaeaea?text=Interstellar",
                    Tags = new List<Tag> { tags["Ulubione"] }
                },
                new Film
                {
                    Title = "Parasite",
                    Year = 2019,
                    Genre = "Dramat",
                    Director = "Bong Joon-ho",
                    Rating = 8.6m,
                    Description = "Chciwość i dyskryminacja klasowa zagrażają nowo powstałej, symbiotycznej relacji między bogatą rodziną Park a ubogą rodziną Kim.",
                    Review = null,
                    WatchedDate = null,
                    IsWatched = false,
                    PosterUrl = "https://placehold.co/300x450/3a2e2e/eaeaea?text=Parasite",
                    Tags = new List<Tag> { tags["Do obejrzenia w weekend"] }
                },
                new Film
                {
                    Title = "Diuna: Część druga",
                    Year = 2024,
                    Genre = "Sci-Fi",
                    Director = "Denis Villeneuve",
                    Rating = 8.5m,
                    Description = "Paul Atryda jednoczy się z Fremenami, by toczyć wojnę o zemstę przeciwko spiskowcom, którzy zniszczyli jego rodzinę.",
                    Review = null,
                    WatchedDate = null,
                    IsWatched = false,
                    PosterUrl = "https://placehold.co/300x450/c97b3c/eaeaea?text=Diuna+2",
                    Tags = new List<Tag> { tags["Premiera"] }
                },
                new Film
                {
                    Title = "Oppenheimer",
                    Year = 2023,
                    Genre = "Biografia",
                    Director = "Christopher Nolan",
                    Rating = 8.4m,
                    Description = "Historia amerykańskiego naukowca J. Roberta Oppenheimera i jego roli w powstaniu bomby atomowej.",
                    Review = null,
                    WatchedDate = null,
                    IsWatched = false,
                    PosterUrl = "https://placehold.co/300x450/2b2b2b/eaeaea?text=Oppenheimer",
                    Tags = new List<Tag> { tags["Premiera"] }
                },
                new Film
                {
                    Title = "Ojciec chrzestny",
                    Year = 1972,
                    Genre = "Kryminał",
                    Director = "Francis Ford Coppola",
                    Rating = 9.2m,
                    Description = "Starzejący się patriarcha rodu mafijnego przekazuje kontrolę nad swoim tajnym imperium niechętnemu synowi.",
                    Review = "Klasyk klasyków.",
                    WatchedDate = new DateTime(2024, 2, 11),
                    IsWatched = true,
                    PosterUrl = "https://placehold.co/300x450/1a1a1a/eaeaea?text=Ojciec+Chrzestny",
                    Tags = new List<Tag> { tags["Klasyka"] }
                }
            };

            foreach (var film in films)
            {
                film.OwnerId = ownerId;
            }

            context.Films.AddRange(films);
            context.SaveChanges();
        }

        private static void BackfillOrphans(ApplicationDbContext context, string? ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
            {
                return;
            }

            var orphans = context.Films.Where(f => f.OwnerId == null).ToList();
            if (orphans.Count == 0)
            {
                return;
            }

            foreach (var film in orphans)
            {
                film.OwnerId = ownerId;
            }
            context.SaveChanges();
        }
    }
}
