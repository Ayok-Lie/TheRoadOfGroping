using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Files
{
    public interface IMinioFileManager : IDomainService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        Task UploadMinio(Stream stream, string file, string contentType);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        Task DeleteMinioFileAsync(RemoveObjectInput input);

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task<ObjectOutPut> GetFile(string filename);
    }
}