using FilmLibraryPP.Models;
using Microsoft.AspNetCore.Identity;

namespace FilmLibraryPP.Data
{
    /// <summary>
    /// Statyczny helper inicjalizujący role aplikacji (<c>Admin</c>, <c>User</c>) oraz konto administratora.
    /// Wywoływany przy starcie aplikacji w <c>Program.cs</c>, po zastosowaniu migracji EF.
    /// </summary>
    public static class IdentitySeed
    {
        /// <summary>Nazwa roli administratora. Sprawdzane w <c>[Authorize(Roles = ...)]</c> oraz w widokach.</summary>
        public const string AdminRole = "Admin";

        /// <summary>Nazwa roli zwykłego użytkownika.</summary>
        public const string UserRole = "User";

        /// <summary>Email konta administratora używany jeśli konfiguracja <c>AdminUser:Email</c> nie jest ustawiona.</summary>
        public const string DefaultAdminEmail = "admin@filmlibrary.local";

        /// <summary>
        /// Tworzy brakujące role i konto admina. Hasło admina pobierane z <c>AdminUser:Password</c> w konfiguracji
        /// (zwykle User Secrets). Jeśli hasła brak — metoda zwraca <c>null</c> bez tworzenia konta.
        /// </summary>
        /// <returns>Utworzone lub istniejące konto admina, albo <c>null</c> jeśli hasło nie jest skonfigurowane.</returns>
        public static async Task<ApplicationUser?> InitializeAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            foreach (var role in new[] { AdminRole, UserRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = configuration["AdminUser:Email"] ?? DefaultAdminEmail;
            var adminPassword = configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                return null;
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Nie udało się utworzyć konta administratora: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }

            return admin;
        }
    }
}
