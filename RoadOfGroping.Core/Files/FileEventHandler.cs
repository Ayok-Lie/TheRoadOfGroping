using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;


namespace RoadOfGroping.Core.Files
{
    public class FileEventHandler : IEventHandle<FileEventDto>, ITransientDependency
    {
        private readonly IMinioService _minioService;
        private readonly MinioConfig _minioOptions;
        private readonly ILogger<FileEventHandler> _logger;

        public FileEventHandler(IMinioService minioService, IOptions<MinioConfig> minioOptions, ILogger<FileEventHandler> logger)
        {
            _minioService = minioService;
            _minioOptions = minioOptions.Value;
            _logger = logger;
        }

        public async Task Handle(FileEventDto eto)
        {
            _logger.LogInformation("FileEventHandler: {0}", eto.File);
            var obj = new UploadObjectInput(_minioOptions.DefaultBucket
                , eto.File
            , eto.ContentType
                , eto.Stream);

            await _minioService.UploadObjectAsync(obj);
        }
    }
}