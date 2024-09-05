namespace RoadOfGroping.Utility.Minio
{
    public class GetFileListInput
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public List<string> PrefixArr { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 是否递归
        /// </summary>

        public bool Recursive { get; set; }
    }
}