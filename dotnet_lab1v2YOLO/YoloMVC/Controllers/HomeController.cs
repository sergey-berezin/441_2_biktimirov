using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
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
            catch (FormatException fe)
            {
                _logger.LogCritical($"The format of base64 image representation is invalid (empty or contains a non-base-64 character): {fe.Message}", fe);
                return StatusCode((int)HttpStatusCode.BadRequest, fe.Message);
            }
            catch (ArgumentNullException ane)
            {
                _logger.LogCritical($"The base64 image representation is null", ane);
                return StatusCode((int)HttpStatusCode.BadRequest, ane.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
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