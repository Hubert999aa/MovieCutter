using Application.Helpers;
using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace JobsRunner.Services
{
    public class VideoProcessorService(ILogger<VideoProcessorService> _logger, IOperationStatusManager _operationStatusManager) : IVideoProcessor
    {
        private static readonly Regex DurationVideoRegex = new(@"Duration:\s(?<duration>\d{2}:\d{2}:\d{2}\.\d+)");
        private static readonly Regex ProgressVideoRegex = new(@"out_time=(?<time>\d{2}:\d{2}:\d{2}\.\d+)");

        public async Task<bool> CutVideoPieceAsync(VideoPiece piece, string sourcePath, string newVideoPathWithoutExtension, string videoExtension, int videoNumber, CancellationToken cancellationToken)
        {
            var processedSuccessfully = true;
            var newVideoPath = $"{newVideoPathWithoutExtension}_{videoNumber}{videoExtension}";
            var messageChannel = Channel.CreateUnbounded<string>();
            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-ss {piece.StartTime} -to {piece.EndTime} -i \"{sourcePath}\" -c copy \"{newVideoPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, cancellationToken);

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]"))
                {
                    _logger.LogWarning(message);

                    if (message.Contains("Error opening input"))
                    {
                        processedSuccessfully = false;
                        break;
                    }
                }
            }

            await processTask;

            return processedSuccessfully;
        }

        public async Task<bool> CutVideoFramesAsync(int operationId, string sourcePath, string outputFolder, string videoName, CancellationToken cancellationToken)
        {
            TimeSpan? totalDuration = null;
            var processedSuccessfully = true;
            var messageChannel = Channel.CreateUnbounded<string>();
            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{sourcePath}\" -fps_mode passthrough -progress pipe:1 -nostats \"{Path.Combine(outputFolder, $"{videoName}_%06d.png")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, cancellationToken);

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]") && (message.Contains("Error parsing") || message.Contains("Error muxing") || message.Contains("Task finished with error")))
                {
                    processedSuccessfully = false;
                    break;
                }

                if (totalDuration == null)
                {
                    var durationMatch = DurationVideoRegex.Match(message);
                    if (durationMatch.Success)
                    {
                        totalDuration = TimeSpan.Parse(durationMatch.Groups["duration"].Value);
                        continue;
                    }
                }

                if (totalDuration.HasValue)
                {
                    var progressMatch = ProgressVideoRegex.Match(message);
                    if (progressMatch.Success)
                    {
                        var current = TimeSpan.Parse(progressMatch.Groups["time"].Value);
                        var progress = (int)Math.Round(current.TotalSeconds * 100 / totalDuration.Value.TotalSeconds);

                        _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.CuttingIntoFrames, progress);
                    }
                }
            }

            await processTask;

            return processedSuccessfully;
        }
    }
}