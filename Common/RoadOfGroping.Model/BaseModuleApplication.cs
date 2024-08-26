using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Model.Interface;
using RoadOfGroping.Model.Modules;

namespace RoadOfGroping.Model
{
    public class BaseModuleApplication : IModuleApplication
    {
        public Type StartModuleType { get; private set; }

        public IServiceCollection Services { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IReadOnlyList<IBaseModuleDescritor> Modules { get; private set; }

        private bool isConfigService;

        public BaseModuleApplication(Type startModuleType, IServiceCollection services)
        {
            var moduleLoader = new ModuleLoad();
            BaseModule.CheckModuleType(startModuleType);
            services.CheckNull();
            StartModuleType = startModuleType;
            Services = services;
            isConfigService = false;

            services.AddSingleton<IModuleLoad>(moduleLoader);

            services.AddObjectAccessor<IServiceProvider>();

            Services.AddObjectAccessor<InitApplicationContext>();

            Services.AddSingleton<IModuleContainer>(this);

            Services.AddSingleton<IModuleApplication>(this);

            Modules = LoadModules(services);

            ConfigerService();
        }

        public virtual void ConfigerService()
        {
            if (isConfigService) return;

            ServiceConfigerContext context = new ServiceConfigerContext(Services);

            foreach (var module in Modules)
            {
                if (module.Instance is BaseModule Module)
                {
                    Module.ServiceConfigerContext = context;
                }
            }

            //PreInitApplication
            try
            {
                foreach (var module in Modules.Where(m => m.Instance is IPreConfigureServices))
                {
                    try
                    {
                        module.Instance.PreConfigureServices(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"期间发生错误 {nameof(IPreConfigureServices.PreConfigureServices)} 模块阶段 {module.ModuleType.AssemblyQualifiedName}.有关详细信息，请参阅内部异常。.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            //ConfigerService
            try
            {
                foreach (var module in Modules)
                {
                    if (module.Instance is BaseModule zModule)
                    {
                        if (true)
                        {
                            //继承生命周期接口的类进行自动注册
                            Services.AddAssembly(module.ModuleType.Assembly);
                        }
                    }
                    try
                    {
                        module.Instance.ConfigerService(context);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"期间发生错误  {nameof(IBaseModule.ConfigerService)}  模块阶段  {module.ModuleType.AssemblyQualifiedName}.有关详细信息，请参阅内部异常。", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            isConfigService = true;
        }

        protected virtual void SetServiceProvider(IServiceProvider servicrovider)
        {
            ServiceProvider = servicrovider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = servicrovider;
        }

        public virtual void InitApplication(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<IModuleManager>()
                .InitializeModulesAsync();
        }

        protected virtual IReadOnlyList<IBaseModuleDescritor> LoadModules(IServiceCollection services)
        {
            return services
                .GetSingletonInstance<IModuleLoad>()
                .GetModuleDescritors(services, StartModuleType);
        }

        public virtual async Task InitApplicationAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            await scope.ServiceProvider
               .GetRequiredService<IModuleManager>()
               .InitializeModulesAsync();
        }
    }
}