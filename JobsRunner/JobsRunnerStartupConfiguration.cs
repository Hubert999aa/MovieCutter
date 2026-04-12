using JobsRunner.Consumers;
using JobsRunner.Options;
using MassTransit;

namespace JobsRunner
{
    public static class JobsRunnerStartupConfiguration
    {
        public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration, RabbitMQExtendedSettings rabbitMqSettings)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<DownloadVideoConsumer>();
                x.AddConsumer<CutVideoIntoPicesConsumer>();
                x.AddConsumer<CutVideoIntoFramesConsumer>();
                x.AddConsumer<DownloadAndCutVideoConsumer>();

                // RabbitMQ configuration
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    // Queue configuration for DownloadVideoConsumer
                    cfg.ReceiveEndpoint(rabbitMqSettings.Queues.DownloadVideoSettings.Name, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(
                            rabbitMqSettings.Queues.DownloadVideoSettings.RetryCount,
                            TimeSpan.FromSeconds(rabbitMqSettings.Queues.DownloadVideoSettings.RetryIntervalSeconds)
                        ));
                        e.PrefetchCount = rabbitMqSettings.Queues.DownloadVideoSettings.PrefetchCount;
                        e.ConfigureConsumer<DownloadVideoConsumer>(context);
                    });

                    // Queue configuration for CutVideoIntoFramesConsumer
                    cfg.ReceiveEndpoint(rabbitMqSettings.Queues.CutVideoIntoFramesSettings.Name, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(
                            rabbitMqSettings.Queues.CutVideoIntoFramesSettings.RetryCount,
                            TimeSpan.FromSeconds(rabbitMqSettings.Queues.CutVideoIntoFramesSettings.RetryIntervalSeconds)
                        ));
                        e.PrefetchCount = rabbitMqSettings.Queues.CutVideoIntoFramesSettings.PrefetchCount;
                        e.ConfigureConsumer<CutVideoIntoFramesConsumer>(context);
                    });

                    // Queue configuration for CutVideoIntoPicesConsumer
                    cfg.ReceiveEndpoint(rabbitMqSettings.Queues.CutVideoIntoPicesSettings.Name, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(
                            rabbitMqSettings.Queues.CutVideoIntoPicesSettings.RetryCount,
                            TimeSpan.FromSeconds(rabbitMqSettings.Queues.CutVideoIntoPicesSettings.RetryIntervalSeconds)
                        ));
                        e.PrefetchCount = rabbitMqSettings.Queues.CutVideoIntoPicesSettings.PrefetchCount;
                        e.ConfigureConsumer<CutVideoIntoPicesConsumer>(context);
                    });

                    // Queue configuration for DownloadAndCutVideoConsumer
                    cfg.ReceiveEndpoint(rabbitMqSettings.Queues.DownloadAndCutVideoSettings.Name, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(
                            rabbitMqSettings.Queues.DownloadAndCutVideoSettings.RetryCount,
                            TimeSpan.FromSeconds(rabbitMqSettings.Queues.DownloadAndCutVideoSettings.RetryIntervalSeconds)
                        ));
                        e.PrefetchCount = rabbitMqSettings.Queues.DownloadAndCutVideoSettings.PrefetchCount;
                        e.ConfigureConsumer<DownloadAndCutVideoConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
