using Application.Mediator;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationInstallation
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMyMediator();

            var rabbitMqSettings = configuration.GetSection("RabbitMQ")
                .Get<RabbitMQBaseSettings>()
                ?? throw new InvalidOperationException("RabbitMQ configuration is missing");

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });
                });
            });

            return services;
        }
    }
}


//ToDo:
// 3. CQRS for Profile
// 4. CQRS for Source

// 6. Test the communication and working requests
// 7. Expose functionalities by controlers endpoints


//Ideas:
// 1. Extend download feature, so we will be able to cut the video into frames straight after downloading