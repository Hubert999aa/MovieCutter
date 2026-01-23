using Domain.ConsumersContracts;
using JobsRunner.Helpers;
using MassTransit;
using System.Diagnostics;

namespace JobsRunner.Consumers
{
    public class CutVideoIntoPicesConsumer(ILogger<CutVideoIntoPicesConsumer> logger) : IConsumer<CutVideoIntoPicesMessage>
    {
        public async Task Consume(ConsumeContext<CutVideoIntoPicesMessage> context)
        {
            logger.LogInformation("Setup cutting into pices process");

            foreach (var pice in context.Message.NewPices)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-ss {pice.StartTime} -to {pice.EndTime} -i \"{context.Message.SourceVideoPath}\" -c copy \"{context.Message.NewVideoPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                await ProcessRunner.RunProcess(startInfo, context.CancellationToken);
            }

            logger.LogInformation("Cutting into pices process finished");
        }
    }
}
