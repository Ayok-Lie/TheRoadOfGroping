using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;

namespace RoadOfGroping.Core.Services
{
    public class TimeNotificationService : ITimeNotificationService, IDefaultDomainService
    {
        private readonly IValuesService _valuesService;
        private ILogger<TimeNotificationService> _logger;

        public TimeNotificationService(IValuesService valuesService, ILogger<TimeNotificationService> logger)
        {
            _valuesService = valuesService;
            //_logger = NullLogger.Instance;
            _logger = logger;
        }

        public IEnumerable<string> FindAll()
        {
            _valuesService.GetValues();
            _logger.LogError("测试日志信息");

            return new[] { "value1", "value2" };
        }

        public string Find(int id)
        {
            //_logger.LogDebug("{method} called with {id}", nameof(this.Find), id);

            return $"value{id}";
        }
    }
}