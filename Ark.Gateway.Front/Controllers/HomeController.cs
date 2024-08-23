using Ark.Gateway.Front.Models;
using Ark.Gateway.Front.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notifyService;
        private readonly IHttpClientService _client;

        public HomeController(IHttpClientService client, ILogger<HomeController> logger, INotyfService notifyService)
        {
            _logger = logger;
            _notifyService = notifyService;
            _client = client;


        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return RedirectToAction("Landing", "Home");
            }

            if (TempData.Peek("Message") != null)
            {
                string? message = TempData?.Peek("Message")?.ToString();
                string? type = TempData?.Peek("Type")?.ToString();
                switch (type)
                {
                    case "Success":
                        _notifyService.Success(message);
                        break;
                    case "Error":
                        _notifyService.Error(message);
                        break;
                    case "Warning":
                        _notifyService.Warning(message);
                        break;
                    case "Information":
                        _notifyService.Information(message);
                        break;
                    default:
                        _notifyService.Information(message);
                        break;
                }
            }
            TempData?.Remove("Message");
            TempData?.Remove("Type");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Landing()
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