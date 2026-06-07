# Biblioteczka Filmowa

Aplikacja webowa do zarządzania prywatną kolekcją filmów —
oznaczanie obejrzanych i tych do zobaczenia, dodawanie własnych
recenzji i ocen, filtrowanie i wyszukiwanie, statystyki w formie
wykresów.

Projekt zaliczeniowy / portfolio.

## Stack technologiczny

- **Backend:** ASP.NET Core MVC, C#, .NET 8.0
- **Frontend:** Razor Views, Bootstrap 5, Bootstrap Icons, Chart.js
- **Baza danych:** PostgreSQL (przez Entity Framework Core + Npgsql) *[planowane]*
- **Autoryzacja:** ASP.NET Core Identity z rolami Admin / User *[planowane]*
- **Zewnętrzne API:** TMDB (The Movie Database) — pobieranie metadanych i plakatów *[planowane]*
- **Testy:** xUnit *[planowane]*
- **Konteneryzacja:** Docker, docker-compose *[planowane]*
- **Dokumentacja:** DocFX *[planowane]*

## Funkcjonalności

### Zrobione
- [x] Lista filmów z miniaturkami plakatów (siatka kart)
- [x] Szczegóły filmu (opis, recenzja, status, tagi)
- [x] Filtrowanie po tytule / reżyserze / gatunku
- [x] Listy „Do obejrzenia" i „Obejrzane"
- [x] Statystyki (4 KPI + 3 wykresy: gatunki, oceny, obejrzane wg roku)
- [x] Spójny UI z monochromatycznymi ikonami

### W planach
- [ ] Persystencja w PostgreSQL (Entity Framework Core)
- [ ] Logowanie, rejestracja, role Admin / User
- [ ] Ręczne dodawanie / edycja / usuwanie filmów (formularze)
- [ ] Integracja z TMDB — wyszukiwanie filmów i pobieranie plakatów
- [ ] Tagi (relacja many-to-many)
- [ ] Eksport listy do CSV
- [ ] REST API z dokumentacją Swagger
- [ ] Testy jednostkowe (xUnit)
- [ ] Dokumentacja techniczna (DocFX)
- [ ] Konteneryzacja (docker-compose: aplikacja + baza)

## Uruchomienie lokalne

Wymagania: .NET 8.0 SDK, opcjonalnie Docker.

```powershell
cd FilmLibraryPP
dotnet restore
dotnet run
```

Aplikacja będzie dostępna pod `https://localhost:7073` lub `http://localhost:5269`.

## Struktura projektu

```
FilmLibraryPP/
├── Controllers/      # HomeController, FilmsController, StatisticsController
├── Models/           # Film
├── Data/             # SampleData (in-memory; tymczasowo, do czasu wpięcia EF Core)
├── Views/            # Razor Views (Home, Films, Statistics, Shared)
├── wwwroot/          # Statyczne assety (CSS, JS, Bootstrap, jQuery)
├── Program.cs
├── Dockerfile
└── FilmLibraryPP.csproj
```

## Licencja

Projekt edukacyjny — bez licencji komercyjnej.
