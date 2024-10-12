using Minio.DataModel;
using Minio.DataModel.Response;
using Minio.DataModel.Result;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Minio
{
    /// <summary>
    /// Minio文件管理的接口，定义了对Minio客户端的操作方法。
    /// </summary>
    public interface IMinioManager
    {
        /// <summary>
        /// 判断指定的存储桶是否存在。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns>如果存在则返回true，否则返回false</returns>
        Task<bool> IsExist(string bucketName);

        /// <summary>
        /// 创建默认存储桶，如果存储桶已存在则不执行任何操作。
        /// </summary>
        /// <returns>异步任务</returns>
        Task CreateDefaultBucket();

        /// <summary>
        /// 创建一个新的存储桶。
        /// </summary>
        /// <param name="bucketName">要创建的存储桶名称</param>
        /// <returns>异步任务</returns>
        Task CreateBucketAsync(string? bucketName);

        /// <summary>
        /// 移除一个指定的存储桶，只有在存储桶存在的情况下才会执行。
        /// </summary>
        /// <param name="bucketName">要移除的存储桶名称</param>
        /// <returns>异步任务</returns>
        Task RemoveBucket(string? bucketName);

        /// <summary>
        /// 删除指定存储桶中的文件对象。
        /// </summary>
        /// <param name="input">删除对象的输入信息</param>
        /// <returns>异步任务</returns>
        Task RemoveObjectAsync(RemoveObjectInput input);

        /// <summary>
        /// 批量删除存储桶中指定的对象。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectNames">要删除的对象名称列表</param>
        /// <returns>异步任务</returns>
        Task BatchRemoveObjectAsync(string bucketName, List<string> objectNames);

        /// <summary>
        /// 获取已签名的对象访问URL，适用于获取对象的临时访问链接。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectNames">对象名称</param>
        /// <returns>返回签名后的URL</returns>
        Task<string> PresignedGetObject(string bucketName, string objectNames);

        /// <summary>
        /// 获取存储桶中指定对象的数据流。
        /// </summary>
        /// <param name="input">对象获取的输入信息</param>
        /// <returns>返回对象的输出信息，包括对象名称和数据流</returns>
        Task<ObjectOutPut> GetObjectAsync(GetObjectInput input);

        /// <summary>
        /// 获取所有现有存储桶的列表。
        /// </summary>
        /// <returns>返回存储桶列表的异步结果</returns>
        Task<ListAllMyBucketsResult?> GetList();

        /// <summary>
        /// 获取存储桶中所有文件的列表。
        /// </summary>
        /// <param name="input">请求文件列表的输入信息</param>
        /// <returns>返回文件列表</returns>
        Task<List<Item>> GetFileList(GetFileListInput input);

        /// <summary>
        /// 下载存储桶中的指定文件到本地路径。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要下载的文件名称</param>
        /// <param name="downloadPath">文件下载到本地的路径</param>
        /// <returns>异步任务</returns>
        Task DownloadFile(string bucketName, string objectName, string downloadPath);

        /// <summary>
        /// 上传指定文件到存储桶。
        /// </summary>
        /// <param name="input">文件上传的输入信息</param>
        /// <returns>返回上传响应结果</returns>
        Task<PutObjectResponse> UploadObjectAsync(UploadObjectInput input);

        /// <summary>
        /// 上传本地文件到默认存储桶。
        /// </summary>
        /// <param name="uploads">要上传的文件列表</param>
        /// <returns>异步任务</returns>
        Task UploadLocalUseDefaultBucket(List<UploadObjectInput> uploads);

        /// <summary>
        /// 删除存储桶中指定的文件。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要删除的文件名称</param>
        /// <returns>异步任务</returns>
        Task DeleteFile(string bucketName, string objectName);

        /// <summary>
        /// 获取指定文件的URL链接，允许临时访问。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要获取链接的文件名称</param>
        /// <returns>返回文件的URL链接</returns>
        Task<string> GetFileUrl(string bucketName, string objectName);
    }
}