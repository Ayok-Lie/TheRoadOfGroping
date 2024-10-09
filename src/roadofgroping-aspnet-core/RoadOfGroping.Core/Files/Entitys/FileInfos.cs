using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Files.Entitys
{
    public class FileInfos : FullAuditedEntity<Guid>
    {
        public FileType _fileType;

        /// <summary>
        /// 文件显示名称，例如文件名为  open.jpg，显示名称为： open_编码规则
        /// </summary>
        public string FileDisplayName { get; set; }

        /// <summary>
        /// 文件原始名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件大小，字节
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType FileType
        {
            get
            {
                return _fileType;
            }
            set
            {
                _fileType = value;
                FileTypeString = _fileType.ToNameValue();
            }
        }

        /// <summary>
        /// 文件类型名称
        /// </summary>
        public string FileTypeString { get; set; }

        /// <summary>
        /// Ip 地址
        /// </summary>
        public string FileIpAddress { get; set; }
    }
}