using RoadOfGroping.Utility.EventBus;

namespace RoadOfGroping.Application.Service.Handler
{
    public class OrderCreatedEventHandler : IntegrationEventHandle<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent @event)
        {
            await Task.Run(() =>
            {
                // 处理订单创建事件逻辑...
                @event.Status = "完成";
                Console.WriteLine($"订单创建事件处理完成，订单ID：{@event.OrderId};订单:{@event.OrderName}{@event.Status}");
            });
        }
    }

    public class WorkOrderCreatedEventHandler : IntegrationEventHandle<WorkOrderCreatedEvent>
    {
        public async Task Handle(WorkOrderCreatedEvent @event)
        {
            await Task.Run(() =>
            {
                // 处理订单创建事件逻辑...
                @event.Status = "完成";
                Console.WriteLine($"工单创建事件处理完成，工单ID：{@event.WorkOrderId};工单:{@event.WorkOrderName}{@event.Status}");
            });
        }
    }
}