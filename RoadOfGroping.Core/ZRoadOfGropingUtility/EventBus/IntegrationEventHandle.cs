namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public interface IntegrationEventHandle<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event);
    }
}