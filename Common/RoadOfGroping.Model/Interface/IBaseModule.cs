namespace RoadOfGroping.Model.Interface
{
    public interface IBaseModule : IPreConfigureServices
    {
        void ConfigerService(ServiceConfigerContext context);

        void InitApplication(InitApplicationContext context);

        void LaterInitApplication(InitApplicationContext context);
    }
}