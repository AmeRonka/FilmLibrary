using Microsoft.AspNetCore.Identity;

namespace FilmLibraryPP.Models
{
    /// <summary>
    /// Konto użytkownika aplikacji. Rozszerza standardowego <see cref="IdentityUser"/> z ASP.NET Core Identity —
    /// na razie bez dodatkowych pól, ale klasa istnieje jako rozszerzalny punkt w przyszłości
    /// (np. avatar, ulubiony gatunek, preferencje UI).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
    }
}
