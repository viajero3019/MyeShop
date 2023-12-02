using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyeShop.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

if (builder.Environment.IsDevelopment())
{
    MyeShop.Infrastructure.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
}

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// app.MapGet("/other", () => "Hello, other");

app.Run();