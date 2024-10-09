using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Common.Attributes;

namespace RoadOfGroping.Host.Controllers
{
    [SkipActionFilter]
    [DisabledUnitOfWork(true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("正在加载首页......");
            return View();
        }
    }
}