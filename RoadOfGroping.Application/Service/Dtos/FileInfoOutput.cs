using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadOfGroping.Core.Files.Entitys;

namespace RoadOfGroping.Application.Service.Dtos
{
    public class FileInfoOutput
    {
        public Guid Id { get; set; }
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
        public FileType FileType { get; set; }

        /// <summary>
        /// 文件类型名称
        /// </summary>
        public string FileTypeString { get; set; }

        /// <summary>
        /// Ip 地址
        /// </summary>
        public string FileIpAddress { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime { get; set; }
    }
}
