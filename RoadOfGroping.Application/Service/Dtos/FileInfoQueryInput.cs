using RoadOfGroping.Common.Pager;

namespace RoadOfGroping.Application.Service.Dtos
{
    public class FileInfoQueryInput : Pagination
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string name { get; set; }
    }
}