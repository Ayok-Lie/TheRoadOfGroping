using RoadOfGroping.Utility.EventBus;

namespace RoadOfGroping.Application.Service.Handler
{
    public class OrderCreatedEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public string? OrderName { get; set; }
    }

    public class WorkOrderCreatedEvent : IntegrationEvent
    {
        public int WorkOrderId { get; set; }

        public string? WorkOrderName { get; set; }
    }
}