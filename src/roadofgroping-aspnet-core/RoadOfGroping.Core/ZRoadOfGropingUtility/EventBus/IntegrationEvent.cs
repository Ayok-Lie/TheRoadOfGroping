namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public abstract class IntegrationEvent
    {
        public string? Status { get; set; }
        public DateTime OccurredOn { get; set; }
    }
}