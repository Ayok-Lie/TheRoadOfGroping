using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class PostInitApplicationModuleLifecycleContributor : ModuleLifecycleContributor
    {
        public override void Initialize(InitApplicationContext context, IBaseModule module)
        {
            (module as ILaterInitApplication)?.LaterInitApplication(context);
        }

        public override async Task InitializeAsync(InitApplicationContext context, IBaseModule module)
        {
            await (module as ILaterInitApplication)?.LaterInitApplicationAsync(context);
        }
    }

    public class OnInitApplicationModuleLifecycleContributor : ModuleLifecycleContributor
    {
        public override void Initialize(InitApplicationContext context, IBaseModule module)
        {
            (module as IInitApplication)?.InitApplication(context);
        }

        public override async Task InitializeAsync(InitApplicationContext context, IBaseModule module)
        {
            await (module as IInitApplication)?.InitApplicationAsync(context);
        }
    }
}