using MyMediator.Interfaces;
using Domain.BusinessModels;
using CoreModels.TechnicalModels;
using Microsoft.Extensions.Options;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Files.Queries.GetVideoListFromDiskQuery
{
    public class GetVideoListFromDiskQueryHandler(IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<GetVideoListFromDiskQuery, BaseResponse>
    {
        public async Task<BaseResponse> Handle(GetVideoListFromDiskQuery request, CancellationToken cancellationToken)
        {
            var directoryPath = _folderPathsOptions.Value.DownloadFolderPath;
            var files = new List<FileMetadata>();

            if (!Directory.Exists(directoryPath)) return new BaseResponse(files);

            files = Directory.GetFiles(directoryPath)
                .Select(path => new FileMetadata
                {
                    Name = Path.GetFileName(path),
                    FullPath = Path.GetFullPath(path)
                })
                .ToList();

            return new BaseResponse(files);
        }
    }
}
