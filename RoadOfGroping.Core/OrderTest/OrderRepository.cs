using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.OrderTest
{
    public class OrderRepository : BasicDomainService<Order, long>, IOrderRepository
    {
        public OrderRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<List<Order>> Get()
        {
            return await QueryAsNoTracking.ToListAsync();
        }
        public async Task AddOrder()
        {
            var order = new Order()
            {
                DateTime = DateTime.UtcNow,
                Name = "Test",
            };
            await CreateAsync(order);
        }

        public override async Task ValidateOnCreateOrUpdate(Order entity)
        {
            await Task.CompletedTask;
        }
    }
}