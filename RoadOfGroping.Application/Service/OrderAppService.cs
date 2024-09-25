using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Repository.Dappers;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    /// <summary>
    /// 不带基础参数的服务类
    /// </summary>
    public class OrderAppService : ApplicationService
    {
        private readonly IOrderManager _orderManager;
        private readonly ILogger<OrderAppService> _logger;

        private readonly IDapperManager<Order> dapperManager;

        public OrderAppService(IServiceProvider serviceProvider, IOrderManager orderManager, ILogger<OrderAppService> logger, IDapperManager<Order> dapperManager) : base(serviceProvider)
        {
            _orderManager = orderManager;
            _logger = logger;
            this.dapperManager = dapperManager;
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


        public async Task ExampleUsage()
        {

            // 2. 异步获取所有用户
            var allUsers = await dapperManager.GetAllAsync();
            Console.WriteLine("All users retrieved:");

            foreach (var user in allUsers)
            {
                Console.WriteLine($"{user.Id}: {user.Name}");
            }

            // 3. 异步根据 ID 获取用户
            var userById = await dapperManager.GetByIdAsync("2");
            Console.WriteLine($"User found by ID: {userById.Name}");

            // 4. 异步更新用户信息
            userById.Name = "John Doe Updated";
            await dapperManager.UpdateAsync(userById);
            Console.WriteLine("User updated.");

            // 5. 异步删除用户
            await dapperManager.DeleteAsync("2");
            Console.WriteLine("User deleted.");

            // 6. 异步查询并返回结果集
            var query = "SELECT * FROM Users WHERE Email = @Email";
            var queriedUsers = await dapperManager.QueryAsync(query, new { Email = "john.doe@example.com" });

            Console.WriteLine("Queried users:");
            foreach (var user in queriedUsers)
            {
                Console.WriteLine($"{user.Id}: {user.Name}");
            }

            // 7. 异步根据条件获取用户
            var condition = "DateTime > @StartDate";
            var usersByCondition = await dapperManager.GetByConditionAsync(condition, new { StartDate = DateTime.Now.AddMonths(-1) });

            Console.WriteLine("Users created in the last month:");
            foreach (var user in usersByCondition)
            {
                Console.WriteLine($"{user.Id}: {user.Name}, {user.DateTime}");
            }

            // 8. 找到特定的用户
            var findQuery = "SELECT * FROM Users WHERE Name = @Name";
            var foundUser = await dapperManager.FindAsync(findQuery, new { Name = "张三" });
            Console.WriteLine(foundUser != null ? $"Found user: {foundUser.Name}" : "User not found.");
        }

    }
}