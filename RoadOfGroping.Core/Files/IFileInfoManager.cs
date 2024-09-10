using Microsoft.AspNetCore.Http;
using RoadOfGroping.Core.Files.Entitys;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Files
{
    public interface IFileInfoManager : IBasicDomainService<FileInfos, Guid>
    {
        Task<string> UploadFileAsync(IFormFile file, string minioName);
    }
}