using Application.Helpers;
using Domain.ConsumersContracts;
using MassTransit;
using Serilog;
using System.Diagnostics;

namespace JobsRunner.Consumers
{
    public class DownloadAndCutVideoConsumer(ILogger<DownloadAndCutVideoConsumer> logger) : IConsumer<DownloadAndCutVideoMessage>
    {
        public async Task Consume(ConsumeContext<DownloadAndCutVideoMessage> context)
        {
            logger.LogInformation("Setup download and cutting process");

            await this.DownloadVideo(context);
            var sourceVideoNameWithExtension = await this.GetVideoNameWithExtension(context);

            if (context.Message.CutVideoInOnePiece)
            {
                var sourceVideoFullPath = Path.Combine(context.Message.VideoOutputFolder, sourceVideoNameWithExtension);
                await this.CutVideoIntoFrames(sourceVideoFullPath, sourceVideoNameWithExtension.Split(".")[0], context);
            }
            else
            {
                await this.CutVideoIntoPiciesAndFrames(context, sourceVideoNameWithExtension);
            }

            logger.LogInformation("Download and cutting finished");
        }

        private async Task DownloadVideo(ConsumeContext<DownloadAndCutVideoMessage> context)
        {
            logger.LogInformation("Download and cutting process - Download started");
            var downloadStartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-o \"%(id)s.%(ext)s\" {context.Message.Url}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = context.Message.VideoOutputFolder,
            };

            await ProcessRunner.RunProcess(downloadStartInfo, context.CancellationToken, true);
            logger.LogInformation("Download and cutting process - Download finished");
        }

        private async Task<string> GetVideoNameWithExtension(ConsumeContext<DownloadAndCutVideoMessage> context)
        {
            var videoNameStartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--print \"%(id)s.%(ext)s\" {context.Message.Url}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            Func<Process, Task<string>> customBehaviour = async (process) =>
            {
                using (var reader = process.StandardOutput)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        Log.Fatal("No output from yt-dlp");
                        throw new Exception("No output from yt-dlp");
                    }

                    return line.Trim();
                }
            };

            return await ProcessRunner.RunProcessWithCustomBehaviour(videoNameStartInfo, context.CancellationToken, customBehaviour);
        }

        private async Task CutVideoIntoPiciesAndFrames(ConsumeContext<DownloadAndCutVideoMessage> context, string sourceVideoNameWithExtension)
        {
            logger.LogInformation("Download and cutting process - Cutting into pieces started");
            var videoNumber = 1;
            var sourceVideoNameWithExtensionArray = sourceVideoNameWithExtension.Split('.');
            var sourceVideoName = sourceVideoNameWithExtensionArray[0];
            var sourceVideoExtension = sourceVideoNameWithExtensionArray[1];

            foreach (var pice in context.Message.NewPices)
            {
                var newVideoPath = Path.Combine(context.Message.VideoOutputFolder, $"{sourceVideoName}_{videoNumber}.{sourceVideoExtension}");
                var newVideoNameWithoutExtension = $"{sourceVideoName}_{videoNumber}";

                var videoPiecesStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-ss {pice.StartTime} -to {pice.EndTime} -i \"{context.Message.VideoOutputFolder + sourceVideoNameWithExtension}\" -c copy \"{newVideoPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                await ProcessRunner.RunProcess(videoPiecesStartInfo, context.CancellationToken, true);
                await this.CutVideoIntoFrames(newVideoPath, newVideoNameWithoutExtension, context);

                videoNumber++;
                logger.LogInformation("Download and cutting process - Cutting into pieces finished");
            }
        }

        private async Task CutVideoIntoFrames(string newVideoPath, string newVideoNameWithoutExtension, ConsumeContext<DownloadAndCutVideoMessage> context)
        {
            logger.LogInformation("Download and cutting process - Cutting into frames started");
            var videoFramesFolderPath = Path.Combine(context.Message.FramesOutputFolder, newVideoNameWithoutExtension);
            Directory.CreateDirectory(videoFramesFolderPath);

            var videoFramesStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{newVideoPath}\" -fps_mode passthrough \"{Path.Combine(videoFramesFolderPath, $"{newVideoNameWithoutExtension}_%06d.png")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            await ProcessRunner.RunProcess(videoFramesStartInfo, context.CancellationToken, true);
            logger.LogInformation("Download and cutting process - Cutting into frames finished");
        }
    }
}
