namespace MyeShop.WebApp.Configuration;

public static class ConfigureCookieSettings
{
    public const int ValidityMinutesPeriod = 60;
    public const string IdentifierCookieName = "MyEshopIdentifier";

    public static IServiceCollection AddCookieSettings(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //TODO need to check that.
            //options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
        });

        services.ConfigureApplicationCookie(options => 
        {
            // options.EventsType = typeof(RevokeAuthenticationEvents)
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(ValidityMinutesPeriod);
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.Cookie = new CookieBuilder {
                Name = IdentifierCookieName,
                IsEssential = true // required for auth to work without explicit user consent; adjust to suit your privacy policy
            };
        });
    
        return services;
    }
}