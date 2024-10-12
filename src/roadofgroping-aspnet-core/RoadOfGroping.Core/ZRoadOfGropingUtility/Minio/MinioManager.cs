using System.Reactive.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;
using Minio.DataModel.Response;
using Minio.DataModel.Result;
using Minio.Exceptions;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Minio
{
    /// <summary>
    /// Minio 文件管理器，提供对 Minio 客户端的各种操作。
    /// </summary>
    public class MinioManager : IMinioManager
    {
        private readonly MinioClient _minioClient;
        private readonly IOptions<MinioConfig> _minioOptions;
        private readonly ILogger<MinioManager> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinioManager"/> class.
        /// </summary>
        /// <param name="minioClient">Minio 客户端实例</param>
        /// <param name="minioOptions">Minio 配置信息</param>
        /// <param name="logger">用于日志记录的 ILogger 实例</param>
        public MinioManager(MinioClient minioClient, IOptions<MinioConfig> minioOptions, ILogger<MinioManager> logger)
        {
            _minioClient = minioClient;
            _minioOptions = minioOptions;
            this.logger = logger;
        }

        /// <summary>
        /// 当存储桶为空时使用默认存储桶。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <exception cref="ArgumentNullException">当存储桶名称为空时抛出此异常</exception>
        private void SetDefaultBucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                bucketName = _minioOptions.Value?.DefaultBucket ?? throw new ArgumentNullException("Minio基础配置默认存储桶为空");
            }
        }

        /// <summary>
        /// 判断存储桶是否存在。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns>如果存储桶存在，则返回 true；否则返回 false</returns>
        public async Task<bool> IsExist(string bucketName)
        {
            try
            {
                BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);
                return await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);
            }
            catch (MinioException e)
            {
                logger.LogError("[Bucket]  Exception: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// 创建默认存储桶，如果存储桶已存在则不执行任何操作。
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task CreateDefaultBucket()
        {
            var config = _minioOptions.Value;
            var defaultBucket = config.DefaultBucket;

            if (string.IsNullOrEmpty(defaultBucket))
            {
                return;
            }

            var bucketArgs = new BucketExistsArgs().WithBucket(defaultBucket);
            if (await _minioClient.BucketExistsAsync(bucketArgs))
            {
                return;
            }

            var makeArgs = new MakeBucketArgs().WithBucket(defaultBucket);
            await _minioClient.MakeBucketAsync(makeArgs);
        }

        /// <summary>
        /// 创建一个新的存储桶。
        /// </summary>
        /// <param name="bucketName">要创建的存储桶名称</param>
        /// <returns>异步任务</returns>
        public async Task CreateBucketAsync(string? bucketName)
        {
            try
            {
                BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);
                if (!await _minioClient.BucketExistsAsync(args))
                {
                    ThrowBucketNotExistisException.ExistsException(bucketName);
                }

                MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
            }
            catch (MinioException e)
            {
                logger.LogError("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// 移除一个存储桶，只有在存储桶存在的情况下才会执行。
        /// </summary>
        /// <param name="bucketName">要移除的存储桶名称</param>
        /// <returns>异步任务</returns>
        public async Task RemoveBucket(string? bucketName)
        {
            try
            {
                BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);
                if (!await _minioClient.BucketExistsAsync(args).ConfigureAwait(false))
                {
                    ThrowBucketNotExistisException.NotExistsException(bucketName);
                }

                RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs().WithBucket(bucketName);
                await _minioClient.RemoveBucketAsync(removeBucketArgs);
            }
            catch (MinioException e)
            {
                logger.LogError("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// 删除存储桶中的文件对象。
        /// </summary>
        /// <param name="input">删除对象的输入信息</param>
        /// <returns>异步任务</returns>
        public async Task RemoveObjectAsync(RemoveObjectInput input)
        {
            SetDefaultBucket(input.BucketName);
            ObjectStat stat = null;

            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName);
                stat = await _minioClient.StatObjectAsync(statObjectArgs);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"{DateTime.Now}:{ex.Message}--{ex.ToString()}");
                ThrowMinioFileExistsException.FileNotExistsException(input.ObjectName);
            }

            RemoveObjectArgs rmArgs = new RemoveObjectArgs()
                .WithBucket(input.BucketName)
                .WithObject(input.ObjectName);

            await _minioClient.RemoveObjectAsync(rmArgs);
        }

        /// <summary>
        /// 批量删除存储桶中指定的对象。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectNames">要删除的对象名称列表</param>
        /// <returns>异步任务</returns>
        public async Task BatchRemoveObjectAsync(string bucketName, List<string> objectNames)
        {
            SetDefaultBucket(bucketName);

            RemoveObjectsArgs rmArgs = new RemoveObjectsArgs()
                .WithBucket(bucketName)
                .WithObjects(objectNames);

            var observable = await _minioClient.RemoveObjectsAsync(rmArgs);

            foreach (var error in observable)
            {
                logger.LogWarning($"{error.Key}文件对象删除失败");
            }
        }

        /// <summary>
        /// 获取已签名的对象访问URL，适用于获取对象的临时访问链接。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectNames">对象名称</param>
        /// <returns>返回签名后的URL</returns>
        public async Task<string> PresignedGetObject(string bucketName, string objectNames)
        {
            var presignedUrl = await _minioClient.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectNames)
                .WithExpiry(100));

            return presignedUrl;
        }

        /// <summary>
        /// 获取存储桶中指定对象的数据流。
        /// </summary>
        /// <param name="input">对象获取的输入信息</param>
        /// <returns>返回对象的输出信息，包括对象名称和数据流</returns>
        public async Task<ObjectOutPut> GetObjectAsync(GetObjectInput input)
        {
            SetDefaultBucket(input.BucketName);

            StatObjectArgs statObjectArgs = new StatObjectArgs()
                .WithBucket(input.BucketName)
                .WithObject(input.ObjectName);

            await _minioClient.StatObjectAsync(statObjectArgs);

            MemoryStream objStream = new MemoryStream();

            GetObjectArgs getObjectArgs = new GetObjectArgs()
                .WithBucket(input.BucketName)
                .WithObject(input.ObjectName)
                .WithCallbackStream((stream) =>
                {
                    if (stream is null)
                    {
                        throw new ArgumentNullException("Minio文件对象流为空");
                    }
                    stream.CopyTo(objStream);
                    objStream.Position = 0;
                });

            var statObj = await _minioClient.GetObjectAsync(getObjectArgs);

            return new ObjectOutPut(statObj.ObjectName, objStream, statObj.ContentType);
        }

        /// <summary>
        /// 获取所有现有存储桶的列表。
        /// </summary>
        /// <returns>返回存储桶列表的异步结果</returns>
        public async Task<ListAllMyBucketsResult?> GetList()
        {
            try
            {
                return await _minioClient.ListBucketsAsync();
            }
            catch (MinioException e)
            {
                logger.LogError("Error occurred: " + e);
                throw;
            }
        }

        /// <summary>
        /// 获取存储桶中所有文件的列表。
        /// </summary>
        /// <param name="input">请求文件列表的输入信息</param>
        /// <returns>返回文件列表</returns>
        public async Task<List<Item>> GetFileList(GetFileListInput input)
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(input.BucketName));
            if (found)
            {
                List<Item> filePathList = new List<Item>();
                if (input.PrefixArr == null || input.PrefixArr.Count == 0)
                {
                    var files = _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
                        .WithBucket(input.BucketName)
                        .WithRecursive(input.Recursive));
                    var filePaths = files.ToBlockingEnumerable();
                    filePathList.InsertRange(filePathList.Count(), filePaths);
                }
                else
                {
                    foreach (string prefix in input.PrefixArr)
                    {
                        var files = _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
                            .WithBucket(input.BucketName)
                            .WithPrefix(prefix)
                            .WithRecursive(input.Recursive));
                        var filePaths = files.ToBlockingEnumerable();
                        filePathList.InsertRange(filePathList.Count(), filePaths);
                    }
                }
                if (!string.IsNullOrEmpty(input.FileName))
                {
                    filePathList = filePathList.Where(d => d.Key.Split('/').Last().Contains(input.FileName)).ToList();
                }
                return filePathList;
            }
            else
            {
                throw new ArgumentNullException("存储桶不存在");
            }
        }

        /// <summary>
        /// 下载存储桶中的指定文件到本地路径。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要下载的文件名称</param>
        /// <param name="downloadPath">文件下载到本地的路径</param>
        /// <returns>异步任务</returns>
        public async Task DownloadFile(string bucketName, string objectName, string downloadPath)
        {
            try
            {
                if (!Directory.Exists(downloadPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(downloadPath);
                    directoryInfo.Create();
                }

                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                await _minioClient.StatObjectAsync(statObjectArgs);

                GetObjectArgs getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(downloadPath + objectName);
                await _minioClient.GetObjectAsync(getObjectArgs);
            }
            catch (MinioException e)
            {
                logger.LogError($"Failure\r\n{e.Message}");
            }
        }

        /// <summary>
        /// 上传指定文件到存储桶。
        /// </summary>
        /// <param name="input">文件上传的输入信息</param>
        /// <returns>返回上传响应结果</returns>
        public async Task<PutObjectResponse> UploadObjectAsync(UploadObjectInput input)
        {
            try
            {
                SetDefaultBucket(input.BucketName);
                bool isExit = await IsExist(input.BucketName);

                if (!isExit)
                {
                    logger.LogError($"{input.BucketName}桶不存在");
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(input.BucketName));
                    logger.LogInformation($"{input.BucketName}桶创建成功");
                }

                PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName)
                    .WithStreamData(input.Stream)
                    .WithContentType(input.ContentType)
                    .WithObjectSize(input.Stream.Length);

                if (_minioOptions.Value.Encryption)
                {
                    Aes aesEncryption = Aes.Create();
                    aesEncryption.KeySize = 256;
                    aesEncryption.GenerateKey();
                    var ssec = new SSEC(aesEncryption.Key);
                    putObjectArgs.WithServerSideEncryption(ssec);
                }
                return await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError($"Failure\r\n{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 上传本地文件到默认存储桶。
        /// </summary>
        /// <param name="uploads">要上传的文件列表</param>
        /// <returns>异步任务</returns>
        public async Task UploadLocalUseDefaultBucket(List<UploadObjectInput> uploads)
        {
            List<Task> tasks = new List<Task>();

            foreach (var item in uploads)
            {
                var task = UploadObjectAsync(item);
                tasks.Add(task);

                if (tasks.Count > 25)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            await Task.WhenAll(tasks); // 确保所有剩余任务都完成
        }

        /// <summary>
        /// 删除存储桶中指定的文件。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要删除的文件名称</param>
        /// <returns>异步任务</returns>
        public async Task DeleteFile(string bucketName, string objectName)
        {
            try
            {
                bool isExit = await IsExist(bucketName);

                if (!isExit)
                {
                    ThrowBucketNotExistisException.NotExistsException(bucketName);
                }

                RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                await _minioClient.RemoveObjectAsync(removeObjectArgs);

                logger.LogInformation($"{bucketName}桶中的{objectName}文件删除成功");
            }
            catch (MinioException e)
            {
                logger.LogError($"Failure\r\n{e.Message}");
            }
        }

        /// <summary>
        /// 获取指定文件的 URL 链接，允许临时访问。
        /// </summary>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">要获取链接的文件名称</param>
        /// <returns>返回文件的 URL 链接</returns>
        public async Task<string> GetFileUrl(string bucketName, string objectName)
        {
            try
            {
                PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(60 * 60 * 24 * 7); // 过期时间设置为一周
                return await _minioClient.PresignedGetObjectAsync(args);
            }
            catch (MinioException e)
            {
                logger.LogError($"Failure\r\n{e.Message}");
                throw;
            }
        }
    }
}