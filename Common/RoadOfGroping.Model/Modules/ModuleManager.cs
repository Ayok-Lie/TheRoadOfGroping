using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class ModuleManager : IModuleManager
    {
        private readonly IModuleContainer _moduleContainer;
        private readonly IEnumerable<IModuleLifecycleContributor> _moduleLifecycleContributors;
        private readonly IObjectAccessor<InitApplicationContext> _applicationContext;

        /// <summary>
        /// 初始化 ModuleManager 实例。
        /// </summary>
        /// <param name="moduleContainer">模块容器</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="options">模块生命周期选项</param>
        /// <param name="applicationContext">应用程序上下文访问器</param>
        public ModuleManager(
            IModuleContainer moduleContainer,
            IServiceProvider serviceProvider,
            IOptions<BaseModuleLifecycleOptions> options,
            IObjectAccessor<InitApplicationContext> applicationContext)
        {
            _moduleContainer = moduleContainer;
            // 从选项中获取生命周期贡献者，并通过服务提供者获取其实例
            _moduleLifecycleContributors = options.Value
                .Contributors
                .Select(serviceProvider.GetRequiredService)
                .Cast<IModuleLifecycleContributor>()
                .ToArray();
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// 初始化所有模块。
        /// </summary>
        public void InitializeModules()
        {
            foreach (var contributor in _moduleLifecycleContributors)
            {
                foreach (var module in _moduleContainer.Modules)
                {
                    try
                    {
                        contributor.Initialize(_applicationContext.Value, module.Instance);
                    }
                    catch (Exception ex)
                    {
                        // 抛出详细的异常信息
                        throw new ArgumentException(
                            $"An error occurred during the initialize {contributor.GetType().FullName} phase of the module {module.ModuleType.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 异步初始化所有模块。
        /// </summary>
        public async Task InitializeModulesAsync()
        {
            foreach (var contributor in _moduleLifecycleContributors)
            {
                foreach (var module in _moduleContainer.Modules)
                {
                    try
                    {
                        contributor.Initialize(_applicationContext.Value, module.Instance);
                        await contributor.InitializeAsync(_applicationContext.Value, module.Instance);
                    }
                    catch (Exception ex)
                    {
                        // 抛出详细的异常信息
                        throw new ArgumentException(
                            $"An error occurred during the initialize {contributor.GetType().FullName} phase of the module {module.ModuleType.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                    }
                }
            }
        }
    }
}