using System.Net;
using Microsoft.AspNetCore.Identity;

namespace MyeShop.Infrastructure.Identity;

public class AppIdentityDbContextSeed
{
    public const string ADMINISTRATORS = "Administrators";
    public const string DEFAULT_PASSWORD = "Pass@word1";

    public static async Task SeedAsync(AppIdentityDbContext appIdentityDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        Console.WriteLine("--> Seeding Users");

        await roleManager.CreateAsync(new IdentityRole(ADMINISTRATORS));

        var defaultUser = new ApplicationUser { UserName = "admin@microsoft.com", Email = "admin@microsoft.com"};
       
        await userManager.CreateAsync(defaultUser, DEFAULT_PASSWORD);

        string adminUserName = "admin@test.com";

        var adminUSer = new ApplicationUser { UserName = adminUserName, Email = adminUserName};

        await userManager.CreateAsync(adminUSer, DEFAULT_PASSWORD);

        adminUSer = await userManager.FindByNameAsync(adminUserName);

        if(adminUSer != null) 
        {
            await userManager.AddToRoleAsync(adminUSer, ADMINISTRATORS);
        }
    }
}