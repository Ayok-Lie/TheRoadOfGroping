using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Common.Pager;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    public class FileAppService : ApplicationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MinioConfig _minioOptions;
        private readonly IFileInfoManager _fileInfoManager;
        private readonly IMinioService _minioService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="webHostEnvironment"></param>
        /// <param name="minioFileManager"></param>
        /// <param name="minioOptions"></param>
        /// <param name="fileInfoManager"></param>
        public FileAppService(
            IServiceProvider serviceProvider,
            IWebHostEnvironment webHostEnvironment,
            IOptions<MinioConfig> minioOptions,
            IFileInfoManager fileInfoManager)
            : base(serviceProvider)
        {
            _webHostEnvironment = webHostEnvironment;
            _minioOptions = minioOptions.Value;
            _fileInfoManager = fileInfoManager;
            _minioService = _minioOptions.Enable ? serviceProvider.GetRequiredService<IMinioService>() : null;
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<UploadFileOutput>> UploadFile(IFormFile file)
        {
            if (file is null or { Length: 0 })
            {
                throw new Exception("请上传文件");
            }
            var url = await _fileInfoManager.UploadFileAsync(file, file.FileName);
            return new List<UploadFileOutput>()
            {
                new() { Name = file.FileName, Url = url }
            };
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFile(string fileUrl)
        {
            //是否开启minio
            if (!_minioOptions.Enable)
            {
                var webrootpath = _webHostEnvironment.WebRootPath;
                string s = Path.Combine(webrootpath, fileUrl);
                var contentType = MimeTypes.GetMimeType(fileUrl);
                var stream = File.OpenRead(s);
                return new FileStreamResult(stream, contentType);
            }
            fileUrl = fileUrl.Replace(_minioOptions.DefaultBucket!.TrimEnd('/'), "");
            var obj = new GetObjectInput()
            {
                ObjectName = fileUrl,
                BucketName = _minioOptions.DefaultBucket
            };

            var output = await _minioService.GetObjectAsync(obj);

            return new FileStreamResult(output.Stream, output.ContentType);
        }

        [HttpPost]
        public async Task<PageResult<FileInfoOutput>> GetPage([FromBody] FileInfoQueryInput input)
        {
            var filelist = await _fileInfoManager
                .QueryAsNoTracking.WhereIf(
                    !string.IsNullOrWhiteSpace(input.name),
                    x =>
                        x.FileDisplayName.Contains(input.name)
                        || x.FileName.Contains(input.name)
                        || x.FilePath.Contains(input.name)
                )
                .OrderByDescending(x => x.CreationTime)
                .Count(out var totalCount)
                .Page(input.PageNo, input.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize);
            return new PageResult<FileInfoOutput>()
            {
                PageNo = input.PageNo,
                PageSize = input.PageSize,
                Rows = ObjectMapper.Map<List<FileInfoOutput>>(filelist),
                Total = (int)totalCount,
                Pages = totalPages,
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _fileInfoManager.FindByIdAsync(id);
            await _fileInfoManager.DeleteAsync(entity);
            await _minioService.RemoveObjectAsync(
                new RemoveObjectInput
                {
                    BucketName = _minioOptions.DefaultBucket,
                    ObjectName = entity.FilePath.Replace(_minioOptions.DefaultBucket, string.Empty)
                }
            );
        }
    }
}