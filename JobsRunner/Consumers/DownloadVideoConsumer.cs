using Domain.ConsumersContracts;
using Application.Helpers;
using MassTransit;
using System.Diagnostics;

namespace JobsRunner.Consumers
{
    public class DownloadVideoConsumer(ILogger<DownloadVideoConsumer> logger) : IConsumer<DownloadVideoMessage>
    {
        public async Task Consume(ConsumeContext<DownloadVideoMessage> context)
        {
            logger.LogInformation("Setup download process");

            var startInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-o \"%(id)s.%(ext)s\" {context.Message.Url}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = context.Message.OutputFolder,
            };

            await ProcessRunner.RunProcess(startInfo, context.CancellationToken);

            logger.LogInformation("Download process finished");
        }
    }
}
