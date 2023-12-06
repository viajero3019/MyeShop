using Microsoft.AspNetCore.Identity;
using MyeShop.Infrastructure.Identity;

namespace MyeShop.WebApp.Extensions;

public static class PrepDbExtensions
{
    public static async void PrepIdentityPopulation(WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var scopeProvider = serviceScope.ServiceProvider;
            try
            {
                app.Logger.LogInformation("Seeding the DB.");

                var userManager = scopeProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scopeProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var IdentityContext = scopeProvider.GetRequiredService<AppIdentityDbContext>();
                await AppIdentityDbContextSeed.SeedAsync(IdentityContext, userManager, roleManager);
            }
            catch (System.Exception ex)
            {
                app.Logger.LogError(ex, "An error occurred seeding the DB.");
                throw;
            }
        }
    }
}