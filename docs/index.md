---
_layout: landing
---

# Biblioteczka Filmowa — dokumentacja techniczna

Wygenerowana automatycznie z komentarzy XML kodu źródłowego za pomocą [DocFX](https://dotnet.github.io/docfx/).

## Stack techniczny

- **Backend:** ASP.NET Core MVC 8, Entity Framework Core 8, Npgsql (PostgreSQL)
- **Autoryzacja:** ASP.NET Core Identity z rolami `Admin` / `User`
- **Zewnętrzne API:** TMDB (The Movie Database) — import metadanych i plakatów
- **Testy:** xUnit + EF Core InMemory
- **REST API:** `/api/films` (dokumentacja Swagger pod `/swagger`)

## Sekcje dokumentacji

- [Wprowadzenie](docs/introduction.md) — krótkie omówienie projektu
- [Pierwsze uruchomienie](docs/getting-started.md) — instrukcja setupu lokalnego
- [API Reference](api/FilmLibraryPP.html) — pełna referencja typów, klas, metod
