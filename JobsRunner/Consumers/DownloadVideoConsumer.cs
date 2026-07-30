using Application.Helpers;
using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace JobsRunner.Consumers
{
    public class DownloadVideoConsumer(ILogger<DownloadVideoConsumer> _logger, IOperationStatusManager _operationStatusManager, IMovieCutterDatabase _databaseContext) : IConsumer<DownloadVideoMessage>
    {
        private static readonly Regex ProgressMessageRegex = new(@"^\[Message\]\s+(?<value>\d+(\.\d+)?)$");
        private bool HasError = false;

        public async Task Consume(ConsumeContext<DownloadVideoMessage> context)
        {
            _operationStatusManager.UpdateOperationStatus(OperationStatus.Processing);

            await this.DownloadVideoNameAndExtension(context);

            if (!this.HasError)
            {
                await this.DownloadVideo(context);
            }

            if (this.HasError)
                _operationStatusManager.UpdateOperationStatus(OperationStatus.Error);
            else
                _operationStatusManager.UpdateOperationStatus(OperationStatus.Finished);
        }

        private async Task DownloadVideoNameAndExtension(ConsumeContext<DownloadVideoMessage> context)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--print \"%(id)s.%(ext)s\" {context.Message.Url}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            var messageChannel = Channel.CreateUnbounded<string>();
            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, context.CancellationToken);

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]"))
                {
                    _logger.LogWarning(message);
                    if (message.Contains("Video unavailable")) this.HasError = true;
                }

                if (message.StartsWith("[Message]"))
                {
                    var messageArray = message.Split("[Message]");
                    var pureMessageArray = messageArray[1].Trim().Split(".");

                    var operation = await _databaseContext.Operations.SingleAsync(p => p.IdOperation == context.Message.Operation.IdOperation);
                    operation.VideoName = pureMessageArray[0];
                    operation.VideoExtension = pureMessageArray[1];
                    await _databaseContext.SaveChangesAsync();
                }
            }

            await processTask;
        }

        private async Task DownloadVideo(ConsumeContext<DownloadVideoMessage> context)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-o \"%(id)s.%(ext)s\" --newline --progress-template \"download:%(progress._percent)s\" {context.Message.Url}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = context.Message.OutputFolder,
            };

            var messageChannel = Channel.CreateUnbounded<string>();
            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, context.CancellationToken);

            await this.HandleDownloadVideoMessageOutput(messageChannel);
            await processTask;
        }

        private async Task HandleDownloadVideoMessageOutput(Channel<string> messageChannel)
        {
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
                        this.HasError = true;
                        continue;
                    }
                }

                if (message.StartsWith("[Message] [Merger]"))
                {
                    _operationStatusManager.UpdateOperationProgress(VideoProcess.Downloading, 99);
                    continue;
                }

                var progressMessageMatch = ProgressMessageRegex.Match(message);
                if (progressMessageMatch.Success)
                {
                    if (double.TryParse(progressMessageMatch.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        var percent = (int)Math.Round((value + 10) * 88); //Video download takes 10%-98%, as 99% is merging and 100% is done
                        _operationStatusManager.UpdateOperationProgress(VideoProcess.Downloading, percent);
                    }
                }
            }
        } 
    }
}