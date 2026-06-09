using FilmLibraryPP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Controllers
{
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FilmsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, string? genre)
        {
            var query = _db.Films.AsQueryable();

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

            ViewBag.Genres = await _db.Films
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
            var films = await _db.Films
                .Where(f => !f.IsWatched)
                .OrderByDescending(f => f.Year)
                .ToListAsync();
            return View(films);
        }

        public async Task<IActionResult> Watched()
        {
            var films = await _db.Films
                .Where(f => f.IsWatched)
                .OrderByDescending(f => f.WatchedDate)
                .ToListAsync();
            return View(films);
        }

        public async Task<IActionResult> Details(int id)
        {
            var film = await _db.Films.FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }
    }
}
