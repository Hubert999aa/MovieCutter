using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMediator;

namespace Application
{
    public static class ApplicationInstallation
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMyMediator();

            return services;
        }
    }
}
