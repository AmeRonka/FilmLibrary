using FilmLibraryPP.Data;
using FilmLibraryPP.Models;
using FilmLibraryPP.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLibraryPP.Controllers.Api
{
    /// <summary>REST API do zarządzania biblioteką filmów użytkownika.</summary>
    [ApiController]
    [Route("api/films")]
    [Authorize]
    [Produces("application/json")]
    public class FilmsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FilmsApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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

        /// <summary>Zwraca paginowaną listę widocznych filmów.</summary>
        /// <param name="page">Numer strony (od 1). Domyślnie 1.</param>
        /// <param name="pageSize">Liczba elementów na stronę (1-100). Domyślnie 20.</param>
        /// <response code="200">Strona wyników.</response>
        [HttpGet]
        [ProducesResponseType(typeof(FilmsPageDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<FilmsPageDto>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            var query = VisibleFilms().OrderByDescending(f => f.Year);
            var totalCount = await query.CountAsync();
            var films = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new FilmsPageDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = films.Select(ToReadDto).ToList()
            });
        }

        /// <summary>Zwraca szczegóły jednego filmu.</summary>
        /// <param name="id">Identyfikator filmu.</param>
        /// <response code="200">Film znaleziony.</response>
        /// <response code="404">Brak filmu o tym ID w widocznym zakresie.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FilmReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FilmReadDto>> GetById(int id)
        {
            var film = await VisibleFilms().FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            return Ok(ToReadDto(film));
        }

        /// <summary>Tworzy nowy film w bibliotece zalogowanego użytkownika.</summary>
        /// <param name="dto">Dane filmu.</param>
        /// <response code="201">Film utworzony. Location header wskazuje na nowy zasób.</response>
        /// <response code="400">Walidacja nie przeszła.</response>
        [HttpPost]
        [ProducesResponseType(typeof(FilmReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FilmReadDto>> Create([FromBody] FilmCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var film = new Film
            {
                Title = dto.Title,
                Year = dto.Year,
                Genre = dto.Genre,
                Director = dto.Director,
                Rating = dto.Rating,
                Description = dto.Description,
                Review = dto.Review,
                WatchedDate = dto.WatchedDate,
                IsWatched = dto.IsWatched,
                PosterUrl = dto.PosterUrl,
                OwnerId = CurrentUserId,
                Tags = await ResolveTagsAsync(dto.Tags, CurrentUserId)
            };

            _db.Films.Add(film);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = film.Id }, ToReadDto(film));
        }

        /// <summary>Aktualizuje istniejący film. Wymaga roli Admin lub bycia właścicielem.</summary>
        /// <param name="id">Identyfikator filmu.</param>
        /// <param name="dto">Nowe dane filmu.</param>
        /// <response code="204">Aktualizacja powiodła się.</response>
        /// <response code="400">Walidacja nie przeszła.</response>
        /// <response code="403">Brak uprawnień.</response>
        /// <response code="404">Brak filmu.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] FilmUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var film = await _db.Films.Include(f => f.Tags).FirstOrDefaultAsync(f => f.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            if (!CanModify(film))
            {
                return Forbid();
            }

            film.Title = dto.Title;
            film.Year = dto.Year;
            film.Genre = dto.Genre;
            film.Director = dto.Director;
            film.Rating = dto.Rating;
            film.Description = dto.Description;
            film.Review = dto.Review;
            film.WatchedDate = dto.WatchedDate;
            film.IsWatched = dto.IsWatched;
            film.PosterUrl = dto.PosterUrl;

            film.Tags.Clear();
            foreach (var tag in await ResolveTagsAsync(dto.Tags, film.OwnerId!))
            {
                film.Tags.Add(tag);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Usuwa film. Wymaga roli Admin lub bycia właścicielem.</summary>
        /// <param name="id">Identyfikator filmu.</param>
        /// <response code="204">Film usunięty.</response>
        /// <response code="403">Brak uprawnień.</response>
        /// <response code="404">Brak filmu.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
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

            _db.Films.Remove(film);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private bool CanModify(Film film)
        {
            return IsAdmin || film.OwnerId == CurrentUserId;
        }

        private static FilmReadDto ToReadDto(Film f) => new FilmReadDto
        {
            Id = f.Id,
            Title = f.Title,
            Year = f.Year,
            Genre = f.Genre,
            Director = f.Director,
            Rating = f.Rating,
            Description = f.Description,
            Review = f.Review,
            WatchedDate = f.WatchedDate,
            IsWatched = f.IsWatched,
            PosterUrl = f.PosterUrl,
            Tags = f.Tags.Select(t => t.Name).ToList(),
            OwnerEmail = f.Owner?.Email
        };

        private async Task<List<Tag>> ResolveTagsAsync(List<string> names, string ownerId)
        {
            if (names == null || names.Count == 0)
            {
                return new List<Tag>();
            }

            var distinct = names
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existing = await _db.Tags
                .Where(t => t.OwnerId == ownerId && distinct.Contains(t.Name))
                .ToListAsync();

            var existingNames = existing.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newTags = distinct
                .Where(n => !existingNames.Contains(n))
                .Select(n => new Tag { Name = n, OwnerId = ownerId })
                .ToList();

            _db.Tags.AddRange(newTags);
            return existing.Concat(newTags).ToList();
        }
    }
}
