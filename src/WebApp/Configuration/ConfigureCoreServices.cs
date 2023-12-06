using MyeShop.ApplicationCore.Interfaces;
using MyeShop.Infrastructure.Services;

namespace MyeShop.WebApp.Configuration;

public static class ConfigureCoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}