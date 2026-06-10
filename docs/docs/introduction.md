# Wprowadzenie

**Biblioteczka Filmowa** to aplikacja webowa do zarządzania prywatną kolekcją filmów —
oznaczanie obejrzanych i tych do zobaczenia, dodawanie własnych recenzji i ocen,
filtrowanie, wyszukiwanie, statystyki w formie wykresów.

## Architektura

Aplikacja typu MVC w ASP.NET Core 8 z następującymi warstwami:

| Warstwa | Klasa / lokalizacja |
|---|---|
| Encje domeny | <xref:FilmLibraryPP.Models.Film>, <xref:FilmLibraryPP.Models.Tag>, <xref:FilmLibraryPP.Models.ApplicationUser> |
| Dostęp do danych | <xref:FilmLibraryPP.Data.ApplicationDbContext> (EF Core + Identity) |
| Kontrolery MVC | `FilmsController`, `StatisticsController`, `HomeController` |
| Kontroler REST API | <xref:FilmLibraryPP.Controllers.Api.FilmsApiController> |
| Serwis zewnętrzny | <xref:FilmLibraryPP.Services.Tmdb.ITmdbService> + <xref:FilmLibraryPP.Services.Tmdb.TmdbMapper> |
| Seedy | <xref:FilmLibraryPP.Data.SeedData>, <xref:FilmLibraryPP.Data.IdentitySeed> |

## Model uprawnień

- **Anonim:** widzi tylko stronę startową; każda chroniona ścieżka przekierowuje na logowanie
- **Zalogowany użytkownik (`User`):** widzi i zarządza wyłącznie własną biblioteką — filmy filtrowane po `OwnerId`
- **Administrator (`Admin`):** widzi wszystkie filmy wszystkich użytkowników z metką „Dodane przez X", może edytować i usuwać dowolne wpisy

## Persystencja

PostgreSQL przez Entity Framework Core. Migracje rezydują w `FilmLibraryPP/Migrations/`.
Schemat obejmuje:

- Tabele aplikacyjne: `Films`, `Tags`, `FilmTags` (join M2M)
- Tabele ASP.NET Core Identity: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`
