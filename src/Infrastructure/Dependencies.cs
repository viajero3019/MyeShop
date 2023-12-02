using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyeShop.Infrastructure.Identity;

namespace MyeShop.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        bool useOnlyInMemoryDatabase = false;

        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
        }

        if(useOnlyInMemoryDatabase)
        {
            services.AddDbContext<AppIdentityDbContext>(context => context.UseInMemoryDatabase("Identity"));
        }
    }
}