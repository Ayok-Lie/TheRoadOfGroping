using RoadOfGroping.Interface.DependencyInjection;

namespace RoadOfGroping.Model.Interface
{
    public interface IModuleLifecycleContributor : ITransientDependency
    {
        void Initialize(InitApplicationContext context, IBaseModule module);

        Task InitializeAsync(InitApplicationContext context, IBaseModule module);
    }
}