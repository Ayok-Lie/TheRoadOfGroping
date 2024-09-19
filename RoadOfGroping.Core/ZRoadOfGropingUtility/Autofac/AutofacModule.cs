using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Autofac;
using RoadOfGroping.Common.Dependency;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.Interceptors;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
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
            foreach (var assembly in assemblies)
            {
                // 注册程序集中的所有类型
                container.RegisterAssemblyTypes(assembly)
                    //.Where(t => t.GetCustomAttribute<AutoFacAop>() != null) 判断是否含有特性
                    .Where(b => !b.IsAbstract && b.IsClass) // 过滤出非抽象类
                    .PublicOnly() // 只注册公共类型
                    .AsImplementedInterfaces(); // 注册为它们实现的接口

                // 遍历程序集中的所有类型
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType))
                {
                    // 如果类型继承自 IScopedDependency
                    if (type.IsAssignableTo<IScopedDependency>())
                    {
                        // 注册为每个生命周期作用域一个实例
                        container.RegisterType(type).InstancePerLifetimeScope();
                    }
                    // 如果类型继承自 ISingletonDependency
                    else if (type.IsAssignableTo<ISingletonDependency>())
                    {
                        // 注册为单例
                        container.RegisterType(type).SingleInstance();
                    }
                    // 如果类型继承自 ITransientDependency
                    else if (type.IsAssignableTo<ITransientDependency>())
                    {
                        // 注册为每次请求一个实例
                        container.RegisterType(type).InstancePerDependency();
                    }
                    else
                    {
                        // 如果没有接口，直接注册实现类
                        container.RegisterType(type).InstancePerLifetimeScope();
                    }
                }
            }

            // 用于Jwt的各种操作
            container.RegisterType<JwtSecurityTokenHandler>().InstancePerLifetimeScope();

            // 支持泛型存入Jwt 便于扩展
            container.RegisterType<TokenHelper>().InstancePerLifetimeScope();

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
}