namespace RoadOfGroping.Core.Services
{
    public interface ITimeNotificationService
    {
        IEnumerable<string> FindAll();

        string Find(int id);
    }
}