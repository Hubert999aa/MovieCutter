using Domain.ConsumersContracts;
using Application.Helpers;
using MassTransit;
using System.Diagnostics;

namespace JobsRunner.Consumers
{
    public class CutVideoIntoFramesConsumer(ILogger<CutVideoIntoFramesConsumer> logger) : IConsumer<CutVideoIntoFramesMessage>
    {
        public async Task Consume(ConsumeContext<CutVideoIntoFramesMessage> context)
        {
            logger.LogInformation("Setup cutting into frames process");

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{context.Message.SourceVideoPath}\" -fps_mode passthrough \"{Path.Combine(context.Message.OutputFolder, $"{context.Message.VideoName}_%06d.png")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            await ProcessRunner.RunProcess(startInfo, context.CancellationToken);

            logger.LogInformation("Cutting into frames process finished");
        }
    }
}
