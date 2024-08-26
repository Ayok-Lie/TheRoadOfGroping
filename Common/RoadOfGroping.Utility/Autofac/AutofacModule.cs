using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Autofac;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Interface.Dependency;
using RoadOfGroping.Utility.ApiResult;
using RoadOfGroping.Utility.AppModel;
using RoadOfGroping.Utility.Token;
using Module = Autofac.Module;

namespace RoadOfGroping.Utility.Autofac
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder container)
        {
            //base.Load(builder);
            Type baseType = typeof(IDependency);
            var basePath = AppContext.BaseDirectory;

            List<Component> components = AppsettingHelper.Get<Component>("Components");
            //foreach (var c in components)
            //{
                //var iServiceDll = $"{basePath}{c.InterfaceName}";
                //var ServiceDll = $"{basePath}{c.ServiceName}";
                //Assembly iServiceAssembly = Assembly.LoadFile(iServiceDll);
                //Assembly serviceAssembly = Assembly.LoadFile(ServiceDll);
                var serviceAssembly = Assembly.Load("RoadOfGroping.Utility");
                container.RegisterAssemblyTypes(serviceAssembly)
                    .Where(b => !b.IsAbstract && baseType.IsAssignableFrom(b))
                    .AsImplementedInterfaces();
            //}

            // 用于Jwt的各种操作
            container.RegisterType<JwtSecurityTokenHandler>().InstancePerLifetimeScope();

            //支持泛型存入Jwt 便于扩展
            container.RegisterType<TokenHelper>().InstancePerLifetimeScope();

            //api返回值处理
            container.RegisterType<ResultHelper>().InstancePerLifetimeScope();

            //container.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerLifetimeScope();
        }
    }
}