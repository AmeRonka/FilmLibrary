using FilmLibraryPP.Data;
using Microsoft.AspNetCore.Mvc;

namespace FilmLibraryPP.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            var films = SampleData.Films;

            var watchedCount = films.Count(f => f.IsWatched);
            var toWatchCount = films.Count(f => !f.IsWatched);

            var byGenre = films
                .Where(f => f.Genre != null)
                .GroupBy(f => f.Genre!)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var byYear = films
                .Where(f => f.IsWatched && f.WatchedDate != null)
                .GroupBy(f => f.WatchedDate!.Value.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderBy(x => x.Year)
                .ToList();

            var ratingBuckets = new[] { "0-5", "5-6", "6-7", "7-8", "8-9", "9-10" };
            var ratingDistribution = ratingBuckets.Select(b =>
            {
                var (min, max) = b switch
                {
                    "0-5" => (0m, 5m),
                    "5-6" => (5m, 6m),
                    "6-7" => (6m, 7m),
                    "7-8" => (7m, 8m),
                    "8-9" => (8m, 9m),
                    _ => (9m, 10.01m)
                };
                return new
                {
                    Bucket = b,
                    Count = films.Count(f => f.Rating >= min && f.Rating < max)
                };
            }).ToList();

            ViewBag.WatchedCount = watchedCount;
            ViewBag.ToWatchCount = toWatchCount;
            ViewBag.ByGenreLabels = byGenre.Select(g => g.Genre).ToArray();
            ViewBag.ByGenreData = byGenre.Select(g => g.Count).ToArray();
            ViewBag.ByYearLabels = byYear.Select(y => y.Year.ToString()).ToArray();
            ViewBag.ByYearData = byYear.Select(y => y.Count).ToArray();
            ViewBag.RatingLabels = ratingDistribution.Select(r => r.Bucket).ToArray();
            ViewBag.RatingData = ratingDistribution.Select(r => r.Count).ToArray();
            ViewBag.TotalCount = films.Count;
            ViewBag.AvgRating = films.Where(f => f.Rating != null).Any()
                ? Math.Round(films.Where(f => f.Rating != null).Average(f => f.Rating!.Value), 1)
                : 0m;

            return View();
        }
    }
}
