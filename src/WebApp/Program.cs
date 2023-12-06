using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyeShop.Infrastructure.Identity;
using MyeShop.WebApp.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();


if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("mailsettings.json");
    MyeShop.Infrastructure.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
}
builder.Services.AddCookieSettings();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCoreServices(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

Console.WriteLine("--> End Build Services");


var app = builder.Build();

app.Logger.LogInformation("--> App created...");




app.UseStaticFiles();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var scopeProvider = scope.ServiceProvider;
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

app.Logger.LogInformation("LAUNCHING");

app.Run();