using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YoloMVC.Models;

namespace YoloMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDetector _detector;
        public HomeController(ILogger<HomeController> logger, IDetector detector) => (_logger, _detector) = (logger, detector);

        [HttpPost]
        [Route("DetectObjects")]
        public async Task<IActionResult> DetectObjects([FromBody] string img64str)
        {
            try
            {
                var imageObjects = await _detector.ProcessImages(Convert.FromBase64String(img64str));
                return Ok(imageObjects);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}