using Domain.ConsumersContracts;
using Application.Helpers;
using MassTransit;
using System.Diagnostics;

namespace JobsRunner.Consumers
{
    public class CutVideoIntoPicesConsumer(ILogger<CutVideoIntoPicesConsumer> logger) : IConsumer<CutVideoIntoPicesMessage>
    {
        public async Task Consume(ConsumeContext<CutVideoIntoPicesMessage> context)
        {
            logger.LogInformation("Setup cutting into pices process");
            var videoNumber = 1;

            foreach (var pice in context.Message.NewPices)
            {
                var newVideoPath = $"{context.Message.NewVideoPathWithoutExtension}_{videoNumber}{context.Message.NewVideoExtension}";
                var startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-ss {pice.StartTime} -to {pice.EndTime} -i \"{context.Message.SourceVideoPath}\" -c copy \"{newVideoPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                await ProcessRunner.RunProcess(startInfo, context.CancellationToken);

                videoNumber++;
            }

            logger.LogInformation("Cutting into pices process finished");
        }
    }
}
