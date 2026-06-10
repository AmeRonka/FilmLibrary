# Pierwsze uruchomienie

## Wymagania

- .NET 8.0 SDK
- PostgreSQL 16 lub nowszy na `localhost:5432`
- (Opcjonalnie) konto na [themoviedb.org](https://www.themoviedb.org/) jeśli chcesz korzystać z importu TMDB

## Setup

1. **Klonowanie i restore:**

   ```powershell
   git clone https://github.com/AmeRonka/FilmLibrary.git
   cd FilmLibrary/FilmLibraryPP
   dotnet restore
   ```

2. **Hasło do bazy w User Secrets** (zamiast trzymać je w plikach):

   ```powershell
   dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Host=localhost;Port=5432;Database=filmlibrary;Username=postgres;Password=TWOJE_HASLO'
   ```

3. **Hasło konta administratora** (admin zostanie utworzony przy starcie):

   ```powershell
   dotnet user-secrets set 'AdminUser:Password' 'Admin123!'
   ```

4. **(Opcjonalne) Token TMDB:**

   ```powershell
   dotnet user-secrets set 'Tmdb:BearerToken' 'eyJ...'
   ```

5. **Uruchomienie:** przy pierwszym starcie EF Core sam utworzy bazę i zasieje dane.

   ```powershell
   dotnet run
   ```

## Testy

```powershell
dotnet test
```

## Dokumentacja Swaggera

Po uruchomieniu aplikacji dostępna pod [`/swagger`](http://localhost:5269/swagger).
