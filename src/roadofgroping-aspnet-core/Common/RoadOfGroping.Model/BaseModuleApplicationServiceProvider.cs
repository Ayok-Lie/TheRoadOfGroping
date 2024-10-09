using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model
{
    /// <summary>
    /// 模块初始化Provider
    /// </summary>
    public class BaseModuleApplicationServiceProvider : BaseModuleApplication, IApplicationServiceProvider
    {
        public BaseModuleApplicationServiceProvider(Type startModuleType, IServiceCollection services) : base(startModuleType, services)
        {
            services.AddSingleton<IApplicationServiceProvider>(this);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Initialize(IServiceProvider serviceProvider)
        {
            SetServiceProvider(serviceProvider);
            InitApplication(serviceProvider);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serviceProvider"></param>
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            SetServiceProvider(serviceProvider);
            await InitApplicationAsync(serviceProvider);
        }
    }
}