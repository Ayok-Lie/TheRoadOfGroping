namespace RoadOfGroping.Model.Interface
{
    public interface IBaseModule : IPreConfigureServices, IInitApplication, ILaterInitApplication
    {
        void ConfigerService(ServiceConfigerContext context);
    }
}