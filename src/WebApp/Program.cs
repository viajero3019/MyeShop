using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyeShop.Infrastructure.Identity;
using MyeShop.WebApp.Configuration;
using MyeShop.WebApp.Extensions;

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


PrepDbExtensions.PrepIdentityPopulation(app);

app.Logger.LogInformation("LAUNCHING");

app.Run();
// PrepDb.PrepPopulation(app, env.IsProduction());