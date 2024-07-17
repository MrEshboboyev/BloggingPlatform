using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BloggingPlatform.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError($"The Path {exceptionHandlerPathFeature?.Path} " +
                $"Threw an Exception {exceptionHandlerPathFeature?.Error}");

            ViewBag.ErrorTitle = TempData["ErrorTitle"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.ErrorDetails = TempData["ErrorDetails"];

            return View("Error");
        }
    }
}
