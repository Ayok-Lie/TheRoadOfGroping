using RoadOfGroping.Common.Enums;

namespace RoadOfGroping.Common.Consts
{
    public class RoadOfGropingConst
    {
        /// <summary>
        /// 多语言源文件地址
        /// </summary>
        public static string LocalizationSourceName = "Localization";

        /// <summary>
        /// 用户id
        /// </summary>
        public const string DefaultUserId = "45D6422E0EBB45DBDC2A08DC86A36122";

        /// <summary>
        /// 角色id
        /// </summary>
        public const string DefaultRoleId = "3F46001E17384276B45808DC89D3BBA8";

        /// <summary>
        /// 用户角色id
        /// </summary>
        public static string DefaultUserRoleId = "A501EB03-344D-44D1-B451-08DC89D3BBA8";

        /// <summary>
        /// 默认用户名
        /// </summary>
        public const string DefaultUserName = "Admin";

        /// <summary>
        /// 默认角色密码
        /// </summary>
        public const string DefaultPassWord = "a2975b35ae9e9801506622bc1703c9fd";

        /// <summary>
        /// 默认权限代码
        /// </summary>
        public const string DefaultRoleCode = "Admin";

        /// <summary>
        /// 默认权限名称
        /// </summary>
        public const string DefaultRoleName = "管理员";

        /// <summary>
        /// 默认用户头像
        /// </summary>
        public const string DefaulUserAvater = "https://www.gravatar.com/avatar/0000000000?d=mp";

        /// <summary>
        /// 默认用户邮箱
        /// </summary>
        public const string DefaultUserEmail = "admin@localhost";

        /// <summary>
        /// 默认用户手机号
        /// </summary>
        public const string DefaultUserPhone = "12138100000";

        /// <summary>
        /// Singlr缓存键
        /// </summary>
        public const string SignlRKey = "signlr.chat.key.";

        public const string DynamicPermisisonFileName = "Permissions.json";
        public static DatabaseType DatabaseType = DatabaseType.SqlServer;
    }
}