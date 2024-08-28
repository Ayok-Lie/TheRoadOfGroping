using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Model.Interface;
using RoadOfGroping.Model.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadOfGroping.Model
{
    public class BaseModuleApplication : IModuleApplication
    {
        public Type StartModuleType { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IReadOnlyList<IBaseModuleDescritor> Modules { get; private set; }

        private bool isConfigService;

        /// <summary>
        /// 初始化 BaseModuleApplication 实例。
        /// </summary>
        /// <param name="startModuleType">启动模块类型</param>
        /// <param name="services">服务集合</param>
        public BaseModuleApplication(Type startModuleType, IServiceCollection services)
        {
            var moduleLoader = new ModuleLoad();
            BaseModule.CheckModuleType(startModuleType);
            services.CheckNull();
            StartModuleType = startModuleType;
            Services = services;
            isConfigService = false;

            services.AddSingleton<IModuleLoad>(moduleLoader);

            // 初始化 ServiceProvider
            ServiceProvider = services.BuildServiceProvider();

            services.AddObjectAccessor<IServiceProvider>();
            Services.AddObjectAccessor<InitApplicationContext>();
            Services.AddSingleton<IModuleContainer>(this);
            Services.AddSingleton<IModuleApplication>(this);

            Modules = LoadModules(services);

            ConfigerService();
        }

        /// <summary>
        /// 配置服务。
        /// </summary>
        public virtual void ConfigerService()
        {
            if (isConfigService) return;

            ServiceConfigerContext context = new ServiceConfigerContext(Services);

            foreach (var module in Modules)
            {
                if (module.Instance is BaseModule baseModule)
                {
                    baseModule.ServiceConfigerContext = context;
                }
            }

            // 预初始化应用程序
            try
            {
                foreach (var module in Modules.Where(m => m.Instance is IPreConfigureServices))
                {
                    try
                    {
                        ((IPreConfigureServices)module.Instance).PreConfigureServices(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"An error occurred during the {nameof(IPreConfigureServices.PreConfigureServices)} phase of the module {module.ModuleType.AssemblyQualifiedName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred during the pre-configuration phase.", ex);
            }

            // 配置服务
            try
            {
                foreach (var module in Modules)
                {
                    if (module.Instance is BaseModule baseModule)
                    {
                        // 继承生命周期接口的类进行自动注册
                        Services.AddAssembly(module.ModuleType.Assembly);
                    }
                    try
                    {
                        module.Instance.ConfigerService(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"An error occurred during the {nameof(IBaseModule.ConfigerService)} phase of the module {module.ModuleType.AssemblyQualifiedName}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred during the configuration phase.", ex);
            }

            isConfigService = true;
        }

        /// <summary>
        /// 设置服务提供者。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        protected virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = serviceProvider;
        }

        /// <summary>
        /// 初始化应用程序。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public virtual void InitApplication(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider
                .GetRequiredService<IModuleManager>()
                .InitializeModules();
        }

        /// <summary>
        /// 异步初始化应用程序。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public virtual async Task InitApplicationAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider
               .GetRequiredService<IModuleManager>()
               .InitializeModulesAsync();
        }

        /// <summary>
        /// 加载模块。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>模块描述符列表</returns>
        protected virtual IReadOnlyList<IBaseModuleDescritor> LoadModules(IServiceCollection services)
        {
            return services
                .GetSingletonInstance<IModuleLoad>()
                .GetModuleDescritors(services, StartModuleType);
        }
    }
}
