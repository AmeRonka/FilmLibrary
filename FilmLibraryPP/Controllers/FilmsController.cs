using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FilmLibraryPP.Data;
using FilmLibraryPP.Models;
using FilmLibraryPP.Services.Tmdb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Controllers
{
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITmdbService _tmdb;

        public FilmsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ITmdbService tmdb)
        {
            _db = db;
            _userManager = userManager;
            _tmdb = tmdb;
        }

        private string CurrentUserId => _userManager.GetUserId(User)!;
        private bool IsAdmin => User.IsInRole(IdentitySeed.AdminRole);

        private IQueryable<Film> VisibleFilms()
        {
            var query = _db.Films.Include(f => f.Tags).AsQueryable();
            if (IsAdmin)
            {
                query = query.Include(f => f.Owner);
            }
            else
            {
                var uid = CurrentUserId;
                query = query.Where(f => f.OwnerId == uid);
            }
            return query;
        }

        public async Task<IActionResult> Index(string? search, string? genre)
        {
            var query = VisibleFilms();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(f =>
                    EF.Functions.ILike(f.Title, pattern) ||
                    (f.Director != null && EF.Functions.ILike(f.Director, pattern)));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(f => f.Genre == genre);
            }

            ViewBag.Genres = await VisibleFilms()
                .Where(f => f.Genre != null)
                .Select(f => f.Genre!)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();
            ViewBag.Search = search;
            ViewBag.Genre = genre;

            return View(await query.OrderByDescending(f => f.Year).ToListAsync());
        }

        public async Task<IActionResult> ToWatch()
        {
            var films = await VisibleFilms()
                .Where(f => !f.IsWatched)
                .OrderByDescending(f => f.Year)
                .ToListAsync();
            return View(films);
        }

        public async Task<IActionResult> Watched()
        {
            var films = await VisibleFilms()
                .Where(f => f.IsWatched)
                .OrderByDescending(f => f.WatchedDate)
                .ToListAsync();
            return View(films);
        }

        public async Task<IActionResult> Details(int id)
        {
            var film = await VisibleFilms().FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Film { Year = DateTime.Today.Year });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Film film, string? tagsCsv)
        {
            film.OwnerId = CurrentUserId;
            ModelState.Remove(nameof(Film.OwnerId));
            ModelState.Remove(nameof(Film.Tags));

            if (!ModelState.IsValid)
            {
                return View(film);
            }

            film.Tags = await ResolveTagsAsync(tagsCsv, CurrentUserId);

            try
            {
                _db.Films.Add(film);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się zapisać filmu: " + ex.Message);
                return View(film);
            }

            return RedirectToAction(nameof(Details), new { id = film.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _db.Films.Include(f => f.Tags).FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            if (!CanModify(film))
            {
                return Forbid();
            }
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Film film, string? tagsCsv)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            var existing = await _db.Films.Include(f => f.Tags).FirstOrDefaultAsync(f => f.Id == id);
            if (existing == null)
            {
                return NotFound();
            }
            if (!CanModify(existing))
            {
                return Forbid();
            }

            ModelState.Remove(nameof(Film.OwnerId));
            ModelState.Remove(nameof(Film.Tags));

            if (!ModelState.IsValid)
            {
                film.OwnerId = existing.OwnerId;
                return View(film);
            }

            existing.Title = film.Title;
            existing.Year = film.Year;
            existing.Genre = film.Genre;
            existing.Director = film.Director;
            existing.Rating = film.Rating;
            existing.Description = film.Description;
            existing.Review = film.Review;
            existing.WatchedDate = film.WatchedDate;
            existing.IsWatched = film.IsWatched;
            existing.PosterUrl = film.PosterUrl;

            existing.Tags.Clear();
            foreach (var tag in await ResolveTagsAsync(tagsCsv, existing.OwnerId!))
            {
                existing.Tags.Add(tag);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się zapisać zmian: " + ex.Message);
                return View(film);
            }

            return RedirectToAction(nameof(Details), new { id = existing.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var film = await _db.Films.FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            if (!CanModify(film))
            {
                return Forbid();
            }
            return View(film);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _db.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            if (!CanModify(film))
            {
                return Forbid();
            }

            try
            {
                _db.Films.Remove(film);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się usunąć filmu: " + ex.Message);
                return View(film);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWatched(int id)
        {
            var film = await _db.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            if (!CanModify(film))
            {
                return Forbid();
            }

            film.IsWatched = true;
            film.WatchedDate ??= DateTime.Today;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ToWatch));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(CancellationToken ct)
        {
            var films = await VisibleFilms()
                .OrderByDescending(f => f.Year)
                .ToListAsync(ct);

            var rows = films.Select(f => new FilmCsvRow
            {
                Tytul = f.Title,
                Rok = f.Year,
                Rezyser = f.Director,
                Gatunek = f.Genre,
                Ocena = f.Rating,
                Obejrzany = f.IsWatched ? "Tak" : "Nie",
                DataObejrzenia = f.WatchedDate?.ToString("yyyy-MM-dd"),
                Tagi = string.Join(" | ", f.Tags.Select(t => t.Name)),
                Opis = f.Description,
                Recenzja = f.Review,
                PlakatUrl = f.PosterUrl
            });

            using var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetPreamble());
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            }))
            {
                await csv.WriteRecordsAsync(rows, ct);
            }

            var fileName = $"filmy-{DateTime.Today:yyyy-MM-dd}.csv";
            return File(stream.ToArray(), "text/csv; charset=utf-8", fileName);
        }

        public class FilmCsvRow
        {
            public string Tytul { get; set; } = string.Empty;
            public int Rok { get; set; }
            public string? Rezyser { get; set; }
            public string? Gatunek { get; set; }
            public decimal? Ocena { get; set; }
            public string Obejrzany { get; set; } = string.Empty;
            public string? DataObejrzenia { get; set; }
            public string? Tagi { get; set; }
            public string? Opis { get; set; }
            public string? Recenzja { get; set; }
            public string? PlakatUrl { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> SearchTmdb(string? query, CancellationToken ct)
        {
            ViewBag.Query = query;
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<TmdbSearchResult>());
            }
            var results = await _tmdb.SearchAsync(query, ct);
            return View(results.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromTmdb(int tmdbId, CancellationToken ct)
        {
            var details = await _tmdb.GetMovieAsync(tmdbId, ct);
            if (details == null)
            {
                TempData["ImportError"] = "Nie udało się pobrać danych filmu z TMDB. Spróbuj ponownie.";
                return RedirectToAction(nameof(SearchTmdb));
            }

            var film = TmdbMapper.ToFilm(details);
            TempData["ImportInfo"] = $"Załadowano dane z TMDB dla „{details.Title}”. Sprawdź pola, w razie potrzeby popraw, i kliknij „Zapisz film”.";
            return View(nameof(Create), film);
        }

        private bool CanModify(Film film)
        {
            return IsAdmin || film.OwnerId == CurrentUserId;
        }

        private async Task<List<Tag>> ResolveTagsAsync(string? csv, string ownerId)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                return new List<Tag>();
            }

            var names = csv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existing = await _db.Tags
                .Where(t => t.OwnerId == ownerId && names.Contains(t.Name))
                .ToListAsync();

            var existingNames = existing
                .Select(t => t.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newTags = names
                .Where(n => !existingNames.Contains(n))
                .Select(n => new Tag { Name = n, OwnerId = ownerId })
                .ToList();

            _db.Tags.AddRange(newTags);

            return existing.Concat(newTags).ToList();
        }
    }
}
