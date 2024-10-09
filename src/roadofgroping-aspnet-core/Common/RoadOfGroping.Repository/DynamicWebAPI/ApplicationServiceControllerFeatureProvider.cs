using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Repository.DynamicWebAPI
{
    /// <summary>
    /// 自定义控制器特性提供程序，用于将实现了 IApplicationService 接口的类识别为控制器。
    /// </summary>
    public class ApplicationServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        /// <summary>
        /// 判断给定的类型是否为控制器。
        /// </summary>
        /// <param name="typeInfo">要判断的类型信息。</param>
        /// <returns>如果类型是控制器，则返回 true；否则返回 false。</returns>
        protected override bool IsController(TypeInfo typeInfo)
        {
            // 检查类型是否实现了 IApplicationService 接口
            if (typeof(IApplicationService).IsAssignableFrom(typeInfo))
            {
                // 检查类型是否满足以下条件：
                // 1. 不是接口
                // 2. 不是抽象类
                // 3. 不是泛型类型
                // 4. 是公共类型
                if (!typeInfo.IsInterface &&
                    !typeInfo.IsAbstract &&
                    !typeInfo.IsGenericType &&
                    typeInfo.IsPublic)
                {
                    return true;
                }
            }

            // 如果不满足上述条件，则返回 false
            return false;
        }
    }
}