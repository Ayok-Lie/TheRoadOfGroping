using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.OrderTest
{
    public interface IOrderManager : IBasicDomainService<Order, long>
    {
        Task<List<Order>> Get();

        Task Create();
    }
}