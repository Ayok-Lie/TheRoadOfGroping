using System.Security.Claims;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos
{
    public class UserAuthDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Roles { get; set; }
    }
}