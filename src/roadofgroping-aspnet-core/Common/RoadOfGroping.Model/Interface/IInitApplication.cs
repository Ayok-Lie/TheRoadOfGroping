namespace RoadOfGroping.Model.Interface
{
    public interface IInitApplication
    {
        void InitApplication(InitApplicationContext context);

        Task InitApplicationAsync(InitApplicationContext context);
    }
}