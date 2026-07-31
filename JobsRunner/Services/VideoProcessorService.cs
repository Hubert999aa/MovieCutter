using Application.Helpers;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using System.Diagnostics;
using System.Threading.Channels;

namespace JobsRunner.Services
{
    public class VideoProcessorService(ILogger<VideoProcessorService> _logger) : IVideoProcessor
    {
        public async Task<bool> CutVideoPieceAsync(VideoPiece piece, string sourcePath, string newVideoPathWithoutExtension, string videoExtension, int videoNumber, CancellationToken cancellationToken)
        {
            var processedSuccessfully = true;
            var newVideoPath = $"{newVideoPathWithoutExtension}_{videoNumber}{videoExtension}";
            var messageChannel = Channel.CreateUnbounded<string>();
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-ss {piece.StartTime} -to {piece.EndTime} -i \"{sourcePath}\" -c copy \"{newVideoPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var processTask = ProcessRunner.RunProcess(startInfo, messageChannel.Writer, cancellationToken);

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
    }
}
