using Application.Helpers;
using Application.Interfaces;
using CoreModels.TechnicalEnums;
using CoreModels.TechnicalModels;
using Domain.BusinessModels;
using Microsoft.EntityFrameworkCore;
using MyMediator.Interfaces;
using Serilog;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace Application.Functions.MovieCutter.Source.Queries.GetSourceLastVideosQuery
{
    public class GetSourceLastVideosQueryHandler(IMovieCutterDatabase _context) : IRequestHandler<GetSourceLastVideosQuery, BaseResponse>
    {
        private const int videosLimit = 5;

        public async Task<BaseResponse> Handle(GetSourceLastVideosQuery request, CancellationToken cancellationToken)
        {
            var sourceBaseUrl = await _context.Sources
                .Where(p => p.IdSource == request.IdSource)
                .Select(p => p.BaseUrl)
                .SingleOrDefaultAsync();

            if (string.IsNullOrEmpty(sourceBaseUrl)) return new BaseResponse(false, ResponseStatus.ValidationError, "No url found");

            var processInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--flat-playlist --dump-json --playlist-end {videosLimit} {sourceBaseUrl}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            var messageChannel = Channel.CreateUnbounded<string>();
            var processTask = ProcessRunner.RunProcess(processInfo, messageChannel.Writer, cancellationToken);
            var videoList = new List<VideoMetadata>();

            await foreach (var message in messageChannel.Reader.ReadAllAsync())
            {
                if (message.StartsWith("[Error]")) Log.Warning(message);
                
                if (message.StartsWith("[Message]"))
                {
                    var messageArray = message.Split("[Message]");
                    using (var jsonDocument = JsonDocument.Parse(messageArray[1]))
                    {
                        var root = jsonDocument.RootElement;
                        var video = new VideoMetadata
                        {
                            Id = root.GetProperty("id").GetString()!,
                            Title = root.GetProperty("title").GetString()!,
                            Url = root.GetProperty("url").GetString()!,
                        };

                        videoList.Add(video);
                    }
                }
            }

            await processTask;
            return new BaseResponse(videoList);
        }
    }
}
