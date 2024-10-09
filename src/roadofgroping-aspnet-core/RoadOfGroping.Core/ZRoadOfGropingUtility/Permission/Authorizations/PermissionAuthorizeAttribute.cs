using Microsoft.AspNetCore.Authorization;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    // 自定义权限授权属性，继承自AuthorizeAttribute
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        // 权限分隔符，用于分隔多个权限
        public const string PermissionSeparator = ",";

        // 策略前缀，用于标识权限策略
        public const string PolicyPrefix = "Permission:";

        // 构造函数，接受多个权限参数
        public PermissionAuthorizeAttribute(params string[] permissions) =>
            Permissions = permissions ?? Array.Empty<string>();

        // 权限数组属性，用于获取和设置权限
        public string[] Permissions
        {
            get
            {
                // 从策略中提取权限，并按分隔符分割
                return Policy[PolicyPrefix.Length..].Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                // 将权限数组转换为字符串，并添加策略前缀
                Policy = $"{PolicyPrefix}{string.Join(PermissionSeparator, value.OrderBy(p => p))}";
            }
        }

        // 尝试从策略名称中获取权限
        public static bool TryGetPermissions(string policyName, out string[] permissions)
        {
            var result = false;
            permissions = null;

            // 检查策略名称是否以策略前缀开头
            if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                result = true;
                // 提取权限并按分隔符分割
                permissions = policyName[PolicyPrefix.Length..]
                    .Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
            }

            return result;
        }
    }
}