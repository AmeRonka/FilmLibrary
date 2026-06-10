# Biblioteczka Filmowa

Aplikacja webowa do zarządzania prywatną kolekcją filmów —
oznaczanie obejrzanych i tych do zobaczenia, dodawanie własnych
recenzji i ocen, filtrowanie i wyszukiwanie, statystyki w formie
wykresów.

Projekt zaliczeniowy / portfolio.

## Stack technologiczny

- **Backend:** ASP.NET Core MVC, C#, .NET 8.0
- **Frontend:** Razor Views, Bootstrap 5, Bootstrap Icons, Chart.js
- **Baza danych:** PostgreSQL 16+ (przez Entity Framework Core 8 + Npgsql)
- **Autoryzacja:** ASP.NET Core Identity z rolami Admin / User
- **Zewnętrzne API:** TMDB (The Movie Database) — wyszukiwanie i import metadanych + plakatów
- **REST API:** własne `/api/films` z dokumentacją Swagger
- **Testy:** xUnit + EF Core InMemory (27 testów)
- **Konteneryzacja:** Docker, docker-compose (aplikacja + Postgres)
- **Dokumentacja:** DocFX (statyczna strona z komentarzy XML)

## Funkcjonalności

- Lista filmów per użytkownik (każdy ma własną bibliotekę)
- Admin widzi wszystkie filmy wszystkich użytkowników
- Szczegóły / edycja / usuwanie z walidacją formularzy
- Tagi (relacja many-to-many) — wpisywane jako CSV
- Filtrowanie po tytule / reżyserze / gatunku, listy „Do obejrzenia" i „Obejrzane"
- Statystyki: 4 KPI + 3 wykresy (gatunki, oceny, obejrzane wg roku)
- Eksport biblioteki do CSV (poprawne kodowanie UTF-8 BOM dla Excela)
- Import filmu z TMDB (wyszukaj → wybierz → pola w formularzu prewypełnione)
- REST API pod `/api/films` (paginacja, CRUD) z dokumentacją Swagger pod `/swagger`

## Uruchomienie w Dockerze (zalecane)

Wymagania: Docker Desktop.

1. **Skopiuj `.env.example` do `.env`** i wypełnij hasła:

   ```powershell
   Copy-Item .env.example .env
   ```

   Edytuj `.env` w edytorze i ustaw `POSTGRES_PASSWORD` oraz `ADMIN_PASSWORD`.

2. **Zbuduj i uruchom:**

   ```powershell
   docker compose up --build
   ```

   Pierwszy raz trwa ~2-3 min (pobranie obrazów + restore pakietów). Kolejne uruchomienia są błyskawiczne.

3. Aplikacja dostępna pod **http://localhost:8080**. Zaloguj się jako:
   - Email: `admin@filmlibrary.local`
   - Hasło: to które ustawiłaś w `.env` jako `ADMIN_PASSWORD`

4. **Zatrzymanie:**

   ```powershell
   docker compose down
   ```

   Dane bazy są w wolumenie `postgres_data` — nie znikają. Pełne wyczyszczenie:
   `docker compose down -v`.

## Uruchomienie lokalne (bez Dockera)

Wymagania: .NET 8.0 SDK, PostgreSQL 16+ uruchomiony lokalnie na porcie 5432.

1. **Hasło do bazy** w User Secrets (w katalogu `FilmLibraryPP/`):

   ```powershell
   dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Host=localhost;Port=5432;Database=filmlibrary;Username=postgres;Password=TWOJE_HASLO'
   ```

2. **Hasło admina i token TMDB:**

   ```powershell
   dotnet user-secrets set 'AdminUser:Password' 'Admin123!'
   dotnet user-secrets set 'Tmdb:BearerToken' 'eyJ...'
   ```

3. **Uruchomienie** (EF Core sam utworzy bazę i zasieje przy pierwszym starcie):

   ```powershell
   dotnet restore
   dotnet run
   ```

Aplikacja będzie dostępna pod `https://localhost:7073` lub `http://localhost:5269`.

## Testy

```powershell
dotnet test
```

## Dokumentacja techniczna (DocFX)

```powershell
dotnet tool install -g docfx   # jednorazowo
docfx docs/docfx.json --serve
```

Otwórz http://localhost:8080.

## Struktura projektu

```
FilmLibraryPP.sln
├── FilmLibraryPP/              # aplikacja MVC
│   ├── Controllers/            # MVC + REST API
│   ├── Models/                 # Film, Tag, ApplicationUser + DTO API
│   ├── Data/                   # ApplicationDbContext, SeedData, IdentitySeed
│   ├── Services/Tmdb/          # ITmdbService, TmdbMapper, DTO
│   ├── Migrations/             # EF Core migracje
│   ├── Views/                  # Razor Views
│   ├── wwwroot/                # statyczne assety
│   ├── Program.cs
│   ├── Dockerfile
│   └── FilmLibraryPP.csproj
├── FilmLibraryPP.Tests/        # xUnit (TmdbMapper, walidacja, M2M, seed)
├── docs/                       # DocFX (źródła + konfiguracja)
├── docker-compose.yml
└── .env.example
```

## Licencja

Projekt edukacyjny — bez licencji komercyjnej.
