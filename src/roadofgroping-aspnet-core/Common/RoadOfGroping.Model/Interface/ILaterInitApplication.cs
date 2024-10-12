namespace RoadOfGroping.Model.Interface
{
    public interface ILaterInitApplication
    {
        void LaterInitApplication(InitApplicationContext context);

        Task LaterInitApplicationAsync(InitApplicationContext context);
    }
}