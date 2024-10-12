using RoadOfGroping.Application;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Model;
using RoadOfGroping.Model.Modules;
using Yitter.IdGenerator;

namespace RoadOfGroping.Host
{
    [DependOn(typeof(RoadOfGropingApplicationModule), typeof(RoadOfGropingEntityFrameworkCoreModule))]
    public class RoadOfGropingHostModule : BaseModule
    {
        public override void ConfigerService(ServiceConfigerContext context)
        {
            context.Services.AddIdGenerator(AppsettingHelper.AppOption<IdGeneratorOptions>("SnowId"));
        }
    }
}