
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.OrderTest
{
    public interface IOrderRepository : IBasicDomainService<Order, long>
    {
        Task<List<Order>> Get();
        Task AddOrder();
    }
}