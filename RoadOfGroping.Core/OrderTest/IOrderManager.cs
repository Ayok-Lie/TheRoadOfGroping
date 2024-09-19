using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.OrderTest
{
    public interface IOrderManager : IAnotherDomainService<Order, long>
    {
        Task<List<Order>> Get();

        Task Create();
    }
}