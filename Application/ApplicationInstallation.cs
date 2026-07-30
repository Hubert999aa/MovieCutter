using Application.Interfaces;
using Application.Services;
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

            services.AddScoped<IOperationStatusManager, OperationStatusManagerService>();

            return services;
        }
    }
}
