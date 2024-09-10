using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    public class OrderAppService : ApplicationService
    {
        private readonly IOrderManager _orderManager;
        private readonly ILogger<OrderAppService> _logger;

        public OrderAppService(IServiceProvider serviceProvider, IOrderManager orderManager, ILogger<OrderAppService> logger) : base(serviceProvider)
        {
            _orderManager = orderManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<Order>> Get(CancellationToken cancellationToken)
        {
            return await _orderManager.Get();
        }

        [HttpPost]
        public async Task Create()
        {
            await _orderManager.Create();
        }
    }
}