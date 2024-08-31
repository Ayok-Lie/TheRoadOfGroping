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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult info()
        {
            return View();
        }

        public IActionResult Index()
        {
            //if (AppSettings.Environment().IsDevelopment())
            //{
            //    ViewBag.Url = "http://localhost:5155";
            //}
            //else
            //{
            //    ViewBag.Url = "http://123.56.30.198:5155";
            //}

            _logger.LogInformation("正在加载首页......");
            return View();
        }
    }
}