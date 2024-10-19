using System.Globalization;

namespace RoadOfGroping.Application.FeatureService.Dtos
{
    public class FeatureListDto
    {
        /// <summary>
        /// 权限
        /// </summary>
        public List<string>? Permissions { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}