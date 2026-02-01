using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Contexts;

namespace Persistance
{
    public static class PersistanceInstallation
    {
        public static IServiceCollection AddPersistanceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MovieCutterContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MovieCutterMSSQL")));

            services.AddScoped<IMovieCutterDatabase>(provider => provider.GetService<MovieCutterContext>());

            return services;
        }
    }
}
