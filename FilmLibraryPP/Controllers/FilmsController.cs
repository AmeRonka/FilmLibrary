using FilmLibraryPP.Data;
using FilmLibraryPP.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmLibraryPP.Controllers
{
    public class FilmsController : Controller
    {
        public IActionResult Index(string? search, string? genre)
        {
            var films = SampleData.Films.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                films = films.Where(f =>
                    f.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (f.Director ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                films = films.Where(f => f.Genre == genre);
            }

            ViewBag.Genres = SampleData.AllGenres;
            ViewBag.Search = search;
            ViewBag.Genre = genre;

            return View(films.OrderByDescending(f => f.Year).ToList());
        }

        public IActionResult ToWatch()
        {
            var films = SampleData.Films
                .Where(f => !f.IsWatched)
                .OrderByDescending(f => f.Year)
                .ToList();
            return View(films);
        }

        public IActionResult Watched()
        {
            var films = SampleData.Films
                .Where(f => f.IsWatched)
                .OrderByDescending(f => f.WatchedDate)
                .ToList();
            return View(films);
        }

        public IActionResult Details(int id)
        {
            var film = SampleData.Films.FirstOrDefault(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }
    }
}
