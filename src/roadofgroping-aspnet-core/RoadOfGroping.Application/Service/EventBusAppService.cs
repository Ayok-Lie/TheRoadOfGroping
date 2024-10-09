using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    /// <summary>
    /// EventBus测试服务
    /// </summary>
    [DisabledUnitOfWork(true)]
    public class EventBusAppService : ApplicationService
    {
        private readonly ILocalEventBus _eventBusServices;

        public EventBusAppService(IServiceProvider serviceProvider, ILocalEventBus eventBusServices) : base(serviceProvider)
        {
            _eventBusServices = eventBusServices;
        }

        //public void PutOrder()
        //{
        //    //创建一个子线程：发布订单到MQ中（发布服务）
        //    Task.Run(() =>
        //    {
        //        // 调用订单服务的方法
        //        for (int i = 0; i < 8; i++)
        //        {
        //            var order = new OrderCreatedEvent { OrderId = i, OrderName = $"测试订单{i}", Status = "下单中" };
        //            _eventBusServices.Publish(order);
        //            Console.WriteLine($"订单创建完成，订单ID：{order.OrderId};订单:{order.OrderName}{order.Status}");
        //            Thread.Sleep(1000);
        //        }
        //    });

        //    //创建一个子线程：发布工单到MQ中（发布服务）
        //    Task.Run(() =>
        //    {
        //        // 调用订单服务的方法
        //        for (int i = 0; i < 8; i++)
        //        {
        //            var workorder = new WorkOrderCreatedEvent { WorkOrderId = i, WorkOrderName = $"测试工单{i}", Status = "下单中" };
        //            _eventBusServices.Publish(workorder);
        //            Console.WriteLine($"工单创建完成，工单ID： {workorder.WorkOrderId} ;工单: {workorder.WorkOrderName}{workorder.Status}");
        //            Thread.Sleep(1000);
        //        }
        //    });
        //}

        public async Task TestLocalEventBus()
        {
            TestDto eto = null;

            for (var i = 0; i < 100; i++)
            {
                eto = new TestDto()
                {
                    Name = "LocalEventBus" + i.ToString(),
                    Description = "幸运值" + i.ToString(),
                };
                await _eventBusServices.PushAsync(eto);
            }
        }

        public async Task EventChnnalCache()
        {
            for (var i = 0; i < 100; i++)
            {
                TestDto eto = new TestDto()
                {
                    Name = "LocalEventBus" + i.ToString(),
                    Description = "幸运值" + i.ToString(),
                };
                await _eventBusServices.EnqueueAsync(eto);
            }
        }
    }
}