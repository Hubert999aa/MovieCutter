using Application.Helpers;
using Application.Interfaces;
using Domain.BusinessEnums;
using JobsRunner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace JobsRunner.Services
{
    public class VideoDownloaderService(ILogger<VideoDownloaderService> _logger, IOperationStatusManager _operationStatusManager, IMovieCutterDatabase _databaseContext) : IVideoDownloader
    {
        private static readonly Regex ProgressMessageRegex = new(@"^\[Message\]\s+(?<value>\d+(\.\d+)?)$");

        public async Task<bool> DownloadVideoAsync(string videoUrl, string outputFolder, int operationId, CancellationToken cancellationToken)
        {
            var downloadedSuccessfully = true;
            var messageChannel = Channel.CreateUnbounded<string>();
            var processInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-o \"%(id)s.%(ext)s\" --newline --progress-template \"download:%(progress._percent)s\" {videoUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = outputFolder,
            };

            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, cancellationToken);

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]"))
                {
                    _logger.LogWarning(message);

                    if (message.Contains("ERROR: Interrupted") ||
                        message.Contains("unable to download video") ||
                        message.Contains("HTTP Error 403: Forbidden") ||
                        message.Contains("You need to log in to access this content"))
                    {
                        downloadedSuccessfully = false;
                        break;
                    }
                }

                if (message.StartsWith("[Message] [Merger]"))
                {
                    _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.Downloading, 99);
                    continue;
                }

                var progressMessageMatch = ProgressMessageRegex.Match(message);
                if (progressMessageMatch.Success)
                {
                    if (double.TryParse(progressMessageMatch.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        var percent = (int)Math.Round((value + 10) * 88); //Video download takes 10%-98%, as 99% is merging and 100% is done
                        _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.Downloading, percent);
                    }
                }
            }

            await processTask;
            return downloadedSuccessfully;
        }

        public async Task<bool> DownloadVideoNameAndExtensionAsync(string videoUrl, int operationId, CancellationToken cancellationToken)
        {
            var downloadedSuccessfully = true;
            var videoName = string.Empty;
            var videoExtension = string.Empty;
            var messageChannel = Channel.CreateUnbounded<string>();
            var processInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--print \"%(id)s.%(ext)s\" {videoUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };
            
            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, cancellationToken);

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]"))
                {
                    _logger.LogWarning(message);
                    if (message.Contains("Video unavailable")) break;
                }

                if (message.StartsWith("[Message]"))
                {
                    var messageArray = message.Split("[Message]");
                    var pureMessageArray = messageArray[1].Trim().Split(".");

                    videoName = pureMessageArray[0];
                    videoExtension = pureMessageArray[1];
                }
            }

            await processTask;

            if (string.IsNullOrEmpty(videoName) || string.IsNullOrEmpty(videoExtension))
            {
                downloadedSuccessfully = false;
            }

            var operation = await _databaseContext.Operations.SingleAsync(p => p.IdOperation == operationId);
            operation.VideoName = videoName;
            operation.VideoExtension = videoExtension;
            await _databaseContext.SaveChangesAsync();
            
            return downloadedSuccessfully;
        }
    }
}
