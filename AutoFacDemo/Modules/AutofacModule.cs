using System.Reflection;
using Autofac;
using RoadOfGroping.Core.Services;

namespace AutoFacDemo.Modules
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<AutoFacAop>();
            //程序集注入业务服务
            var IAppServices = Assembly.Load("RoadOfGroping.Core");
            var AppServices = Assembly.Load("RoadOfGroping.Core");
            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(AppServices)
              .Where(t => t.Name.EndsWith("Service"))
              .Where(t => typeof(IDefaultDomainService).IsAssignableFrom(t))
              //.Where(t => t.GetCustomAttribute<AutoFacAop>() != null)判断是否含有特性
              //.PublicOnly()//只要public访问权限的
              .Where(t => t.IsClass)
              //.PropertiesAutowired()
              .AsImplementedInterfaces() //方法会将每个选定的类注册为它们实现的所有接口
              .InstancePerDependency();
            //builder.Register(c => new TimeNotificationService(c.Resolve<ILogger<TimeNotificationService>>()))
            //    .As<ITimeNotificationService>()
            //    .InstancePerLifetimeScope();
        }
    }
}