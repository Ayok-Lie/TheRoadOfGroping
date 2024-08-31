namespace RoadOfGroping.Utility.EventBus
{
    public interface IEventHandle<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event);
    }
}