using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.Files.Entitys;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Files
{
    public class FileInfoManager : BasicDomainService<FileInfos, Guid>, IFileInfoManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MinioConfig _minioOptions;
        private readonly ILocalEventBus _localEvent;

        public FileInfoManager(IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment,
            IOptions<MinioConfig> minioOptions
,
            ILocalEventBus localEvent) : base(serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _minioOptions = minioOptions.Value;
            _localEvent = localEvent;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string minioName)
        {
            string extension = Path.GetExtension(file.FileName);
            string danymicPath = $"Minio_{Guid.NewGuid().ToString("N")}/";
            // 文件完整名称
            var now = DateTime.Today;
            string filePath = GetTargetDirectory(file.ContentType, $"/{now.Year}-{now.Month:D2}/");
            var fileUrl = $"{filePath}{danymicPath}{minioName}";
            var request = _httpContextAccessor.HttpContext!.Request;
            var fileinfo = new FileInfos()
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FilePath = string.Concat(_minioOptions.DefaultBucket!.TrimEnd('/'), fileUrl),
                FileSize = file.Length.ToString(),
                FileExt = extension,
                FileType = GetFileTypeFromContentType(file.ContentType),
                FileDisplayName = file.FileName.Replace(extension, ""),
            };

            if (!_minioOptions.Enable)
            {
                filePath = string.Concat(_minioOptions.DefaultBucket!.TrimEnd('/'), filePath);
                var webrootpath = _webHostEnvironment.WebRootPath;
                string s = Path.Combine(webrootpath, filePath, danymicPath);
                if (!Directory.Exists(s))
                {
                    Directory.CreateDirectory(s);
                }
                fileinfo.FileIpAddress = $"{request.Scheme}://{request.Host.Value}";
                await CreateAsync(fileinfo);
                var stream = File.Create($"{s}{minioName}");
                await file.CopyToAsync(stream);
                await stream.DisposeAsync();
                fileUrl = string.Concat(_minioOptions.DefaultBucket.TrimEnd('/'), fileUrl);
                string url = $"{request.Scheme}://{request.Host.Value}/api/Files/GetFile?fileUrl={filePath}{danymicPath}{minioName}";
                return url;
            }
            fileinfo.FileIpAddress = $"{request.Scheme}://{_minioOptions.Host!.TrimEnd('/')}";
            await CreateAsync(fileinfo);
            await _localEvent.PushAsync(
                new FileEventDto(file.OpenReadStream(), fileUrl, file.ContentType)
            );
            return $"{request.Scheme}://{_minioOptions.Host!.TrimEnd('/')}/{string.Concat(_minioOptions.DefaultBucket!.TrimEnd('/'), fileUrl)}";
        }

        private static FileType GetFileTypeFromContentType(string contentType)
        {
            // 根据 Content-Type 判断文件类型
            // 这只是一个简单的示例，实际上可能需要更复杂的逻辑
            if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Image;
            }
            else if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Video;
            }
            else
            {
                return FileType.File;
            }
        }

        // 根据文件类型获取目标目录
        private static string GetTargetDirectory(string contentType, string filePath)
        {
            //文件路径
            var fileType = GetFileTypeFromContentType(contentType);

            switch (fileType)
            {
                case FileType.Image:
                    return $"/Image{filePath}"; // 替换成实际的目录路径
                case FileType.Video:
                    return $"/Video{filePath}"; // 替换成实际的目录路径
                default:
                    return $"/File{filePath}"; // 替换成实际的目录路径
            }
        }

        public override Task ValidateOnCreateOrUpdate(FileInfos entity)
        {
            return Task.CompletedTask;
        }
    }
}