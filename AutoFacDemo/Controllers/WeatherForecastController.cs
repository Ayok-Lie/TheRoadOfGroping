using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.Services;

namespace AutoFacDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ITimeNotificationService timeNotificationService;
        private readonly IValuesService valuesService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITimeNotificationService timeNotificationService, IValuesService valuesService)
        {
            _logger = logger;
            this.timeNotificationService = timeNotificationService;
            this.valuesService = valuesService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            valuesService.GetValues();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public IEnumerable<string> GetLogs()
        {
            return timeNotificationService.FindAll();
        }
    }
}