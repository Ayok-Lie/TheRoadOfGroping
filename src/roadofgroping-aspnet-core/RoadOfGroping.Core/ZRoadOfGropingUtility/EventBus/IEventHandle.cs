namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public interface IEventHandle<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event);
    }
}