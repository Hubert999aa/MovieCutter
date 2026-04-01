using Application.Interfaces;
using MyMediator.Interfaces;
using Domain.BusinessModels;
using Domain.TechnicalModels;
using Application.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using Domain.TechnicalEnums;

namespace Application.Functions.MovieCutter.Source.Queries.GetSourceLastVideosQuery
{
    public class GetSourceLastVideosQueryHandler(IMovieCutterDatabase _context) : IRequestHandler<GetSourceLastVideosQuery, BaseResponse>
    {
        private const int videosLimit = 15;

        public async Task<BaseResponse> Handle(GetSourceLastVideosQuery request, CancellationToken cancellationToken)
        {
            var sourceBaseUrl = await _context.Sources
                .Where(p => p.IdSource == request.IdSource)
                .Select(p => p.BaseUrl)
                .SingleOrDefaultAsync();

            if (string.IsNullOrEmpty(sourceBaseUrl))
            {
                return new BaseResponse(false, ResponseStatus.ValidationError, "No url found");
            }

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

            Func<Process, Task<List<VideoMetadata>>> customBehaviour = async (process) =>
            {
                var videos = new List<VideoMetadata>();

                using (var reader = process.StandardOutput)
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        using (var jsonDocument = JsonDocument.Parse(line))
                        {
                            var root = jsonDocument.RootElement;
                            var video = new VideoMetadata
                            {
                                Id = root.GetProperty("id").GetString()!,
                                Title = root.GetProperty("title").GetString()!,
                                Url = root.GetProperty("url").GetString()!,
                            };

                            videos.Add(video);
                        }
                    }
                }

                return videos;
            };

            var videoList = await ProcessRunner.RunProcessWithCustomBehaviour(processInfo, cancellationToken, customBehaviour);
            return new BaseResponse(videoList);
        }
    }
}
