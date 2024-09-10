using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.OrderTest
{
    public class OrderManager : BasicDomainService<Order, long>, IOrderManager
    {
        public OrderManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<Order>> Get()
        {
            return await QueryAsNoTracking.ToListAsync();
        }

        public async Task Create()
        {
            var order = new Order()
            {
                Id = new Random().Next(),
                DateTime = DateTime.UtcNow,
                Name = GenerateRandomChineseCharacter().ToString() + GenerateRandomChineseCharacter().ToString(),
            };
            await CreateAsync(order);
        }

        public override async Task ValidateOnCreateOrUpdate(Order entity)
        {
            await Task.CompletedTask;
        }

        private static char GenerateRandomChineseCharacter()
        {
            // 中文汉字在Unicode编码中的范围是 0x4E00 到 0x9FA5
            int unicode = new Random().Next(0x4E00, 0x9FA6);
            return (char)unicode;
        }
    }
}