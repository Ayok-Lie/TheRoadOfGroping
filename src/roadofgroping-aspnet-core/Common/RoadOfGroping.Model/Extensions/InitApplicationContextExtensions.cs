using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Model.Interface;
using RoadOfGroping.Model.Modules;

namespace RoadOfGroping.Model.Extensions
{
    public static class InitApplicationContextExtensions
    {
        public static void CheckNull(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException("IAppBuilder为空");
        }

        public static IApplicationBuilder GetApplicationBuilder(this InitApplicationContext context)
        {
            return context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;
        }

        public static void InitApplication(this IApplicationBuilder app)
        {
            InitBaseSetting(app);
            var runner = app.ApplicationServices.GetRequiredService<IApplicationServiceProvider>();
            runner.Initialize(app.ApplicationServices);
        }

        public static async Task InitApplicationAsync(this IApplicationBuilder app)
        {
            InitBaseSetting(app);
            var runner = app.ApplicationServices.GetRequiredService<IApplicationServiceProvider>();
            await runner.InitializeAsync(app.ApplicationServices);
        }

        /// <summary>
        /// 初始化基础
        /// </summary>
        /// <param name="app"></param>
        private static void InitBaseSetting(IApplicationBuilder app)
        {
            app.CheckNull();
            app.ApplicationServices.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;
            app.ApplicationServices.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value = app;
        }
    }
}