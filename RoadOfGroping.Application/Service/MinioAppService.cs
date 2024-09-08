using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Repository.DomainService;
using RoadOfGroping.Utility.Minio;

namespace RoadOfGroping.Application.Service
{
    [DisabledUnitOfWork(true)]
    public class MinioAppService : ApplicationService
    {
        private readonly IMinioService minioService;

        public MinioAppService(IServiceProvider serviceProvider,IMinioService minioService): base(serviceProvider)
        {
            this.minioService = minioService;
        }

        public async Task<FileResult> GetMinioObject(GetObjectInput input)
        {
            var data = await minioService.GetObjectAsync(input);
            var fileStreamResult = new FileStreamResult(data.Stream, data.ContentType)
            {
                FileDownloadName = data.Name // 设置你想要下载的文件名
            };
            return fileStreamResult;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="files">文件</param>
        /// <returns></returns>
        public async Task UploadFile(string filePath, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                string saveFileName = $"{Path.GetFileName(formFile.FileName)}";
                var input = new UploadObjectInput();
                input.BucketName = "default-host";
                input.ObjectName = $"/{filePath}/{saveFileName}";//文件保存路径;
                input.ContentType = formFile.ContentType;
                input.Stream = formFile.OpenReadStream();
                await minioService.UploadObjectAsync(input);
            }
        }

        public async Task<List<Item>> GetFileList(GetFileListInput input)
        {
            return await minioService.GetFileList(input);
        }
    }
}