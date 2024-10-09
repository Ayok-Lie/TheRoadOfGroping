namespace RoadOfGroping.Model.Interface
{
    public interface IModuleManager
    {
        void InitializeModules();

        Task InitializeModulesAsync();
    }
}