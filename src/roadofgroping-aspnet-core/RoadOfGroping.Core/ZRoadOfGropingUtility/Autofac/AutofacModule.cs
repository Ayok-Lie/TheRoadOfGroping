using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.Dependency;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.Interceptors;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;
using Module = Autofac.Module;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Autofac
{
    public class AutofacModule : Module
    {
        /// <summary>
        /// 加载模块并注册服务。
        /// </summary>
        /// <param name="container">Autofac 容器构建器。</param>
        protected override void Load(ContainerBuilder container)
        {
            // 定义基础类型
            Type baseType = typeof(IDependency);

            // 获取应用程序的基目录
            var basePath = AppContext.BaseDirectory;

            // 获取所有程序集，并筛选出以 "RoadOfGroping" 开头的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith("RoadOfGroping")).ToArray();

            // 注册 AutoFacAop
            container.RegisterType<AutoFacAop>();

            // 遍历所有程序集
            //foreach (var assembly in assemblies)
            //{
            //    // 注册程序集中的所有类型
            //    //container.RegisterAssemblyTypes(assembly)
            //    //    //.Where(t => t.GetCustomAttribute<AutoFacAop>() != null) 判断是否含有特性
            //    //    .Where(b => !b.IsAbstract && b.IsClass && !b.IsGenericType) // 过滤出非抽象类
            //    //    .PublicOnly() // 只注册公共类型
            //    //    .AsImplementedInterfaces(); // 注册为它们实现的接口

            //    foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType))
            //    {
            //        // 获取类型的接口
            //        var interfaces = type.GetInterfaces();

            //        // 首先判断实现的生命周期并注册
            //        if (type.IsAssignableTo<IScopedDependency>())
            //        {
            //            // 对于作用域依赖，注册实现类和接口
            //            container.RegisterType(type).AsSelf().As(interfaces).InstancePerLifetimeScope();
            //        }
            //        else if (type.IsAssignableTo<ISingletonDependency>())
            //        {
            //            // 对于单例依赖，注册实现类和接口
            //            container.RegisterType(type).AsSelf().As(interfaces).SingleInstance();
            //        }
            //        else if (type.IsAssignableTo<ITransientDependency>())
            //        {
            //            // 对于瞬态依赖，注册实现类和接口
            //            container.RegisterType(type).AsSelf().As(interfaces).InstancePerDependency();
            //        }
            //    }
            //}

            // 用于Jwt的各种操作
            container.RegisterType<JwtSecurityTokenHandler>().InstancePerLifetimeScope();

            // 注册 FilterActionResultWrapFactory 并将其注册为 IActionResultWrapFactory 的实现
            container.RegisterType<FilterActionResultWrapFactory>().As<IActionResultWrapFactory>().InstancePerLifetimeScope();

            // 注册泛型服务
            //container.RegisterAssemblyOpenGenericTypes().InstancePerLifetimeScope();
        }

        private void RegisterGenericTypes(ContainerBuilder builder, IEnumerable<Assembly> assemblys)
        {
            var allTypes = assemblys.Where(assembly => !assembly.IsDynamic)
                         .SelectMany(assembly => assembly.GetExportedTypes())
                         .ToList();

            allTypes = allTypes.Where(t => t.IsClass && !t.IsAbstract
            && t.IsGenericTypeDefinition
            && typeof(ITransientDependency).IsAssignableFrom(t)).ToList();

            foreach (var type in allTypes)
            {
                builder.RegisterGeneric(type);
            }
        }

        ///// <summary>
        ///// 属性注入
        ///// </summary>
        ///// <param name="builder"></param>
        //private void RegisterAutowired(ContainerBuilder builder)
        //{
        //    // Register your own things directly with Autofac, like:
        //    builder.RegisterType<HelloService>().As<IHelloService>().InstancePerDependency().AsImplementedInterfaces();

        //    // 获取所有控制器类型并使用属性注入
        //    var controllerBaseType = typeof(ControllerBase);
        //    builder.RegisterAssemblyTypes(typeof(Program).Assembly)
        //        .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
        //        .PropertiesAutowired(new AutowiredPropertySelector());
        //}

        /// <summary>
        /// 属性注入选择器
        /// </summary>
        public class AutowiredPropertySelector : IPropertySelector
        {
            public bool InjectProperty(PropertyInfo propertyInfo, object instance)
            {
                // 带有 AutowiredAttribute 特性的属性会进行属性注入
                return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(AutowiredAttribute));
            }
        }

        //public class AutofacModule : Autofac.Module
        //{
        //    protected override void Load(ContainerBuilder builder)
        //    {
        //        //builder.RegisterType<AutoFacAop>();
        //        //程序集注入业务服务
        //        var IAppServices = Assembly.Load("RoadOfGroping.Core");
        //        var AppServices = Assembly.Load("RoadOfGroping.Core");
        //        //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
        //        builder.RegisterAssemblyTypes(AppServices)
        //          .Where(t => t.Name.EndsWith("Service"))
        //          .Where(t => typeof(IDefaultDomainService).IsAssignableFrom(t))
        //          //.Where(t => t.GetCustomAttribute<AutoFacAop>() != null)判断是否含有特性
        //          //.PublicOnly()//只要public访问权限的
        //          .Where(t => t.IsClass)
        //          //.PropertiesAutowired()
        //          .AsImplementedInterfaces() //方法会将每个选定的类注册为它们实现的所有接口
        //          .InstancePerDependency();
        //        //builder.Register(c => new TimeNotificationService(c.Resolve<ILogger<TimeNotificationService>>()))
        //        //    .As<ITimeNotificationService>()
        //        //    .InstancePerLifetimeScope();
        //    }
        //}
    }

    public static class AutoDependencyInjection
    {
        public static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            // 获取所有程序集，并筛选出以 "RoadOfGroping" 开头的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith("RoadOfGroping")).ToArray();

            // 遍历符合条件的程序集
            foreach (var assembly in assemblies)
            {
                // 遍历符合条件的类并注册其实现到服务容器中
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType);

                foreach (var type in types)
                {
                    // 获取类型的接口
                    var interfaces = type.GetInterfaces();

                    // 根据生命周期，注册实现类和接口
                    if (type.IsAssignableTo<IScopedDependency>())
                    {
                        RegisterServices(services, interfaces, type, ServiceLifetime.Scoped);
                    }
                    else if (type.IsAssignableTo<ISingletonDependency>())
                    {
                        RegisterServices(services, interfaces, type, ServiceLifetime.Singleton);
                    }
                    else if (type.IsAssignableTo<ITransientDependency>())
                    {
                        RegisterServices(services, interfaces, type, ServiceLifetime.Transient);
                    }
                }
            }

            IOC_DependencyInjectionManager.Current = services.BuildServiceProvider();
            return services;
        }

        private static void RegisterServices(IServiceCollection services, Type[] interfaces, Type implementation, ServiceLifetime lifetime)
        {
            foreach (var @interface in interfaces)
            {
                // 检查是否已经注册
                var existingService = services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == @interface);
                if (existingService != null)
                {
                    // 如果已经注册，则跳过
                    continue;
                }

                // 注册服务
                switch (lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(@interface, implementation);
                        break;

                    case ServiceLifetime.Singleton:
                        services.AddSingleton(@interface, implementation);
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient(@interface, implementation);
                        break;
                }
            }
        }
    }
}