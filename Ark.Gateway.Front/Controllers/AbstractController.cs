using Ark.Gateway.Front.Data.FileManager;
using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class AbstractController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly IFileManager _fm;
        private readonly INotyfService _notifyService;
        private readonly string _abstract;

        public AbstractController(IHttpClientService client, INotyfService notifyService, IFileManager fm, IConfiguration configuration)
        {
            _client = client;
            _fm = fm;
            _notifyService = notifyService;
            _abstract = configuration["Abstract"];
        }

        public async Task<IActionResult> Index()
        {

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

            ViewBag.Url = _abstract;

            List<AbstractViewModel> Abstracts = await _client.GetRequest(new List<AbstractViewModel>(), $"Abstract/GetAbstracts");
            var result = Abstracts.OrderBy(a => a.CreatedOn);
            return View(result);
        }
        public async Task<IActionResult> Abstract(Guid id)
        {
            AbstractViewModel result = await _client.GetRequest(new AbstractViewModel(), $"Abstract/GetAbstract?Abstract={id}");
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
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
            ViewBag.Url = _abstract;
            AbstractViewModel result = await _client.GetRequest(new AbstractViewModel(), $"Abstract/GetAbstract?Abstract={id}");
            return View(result);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            AbstractViewModel item = await _client.GetRequest(new AbstractViewModel(), $"Abstract/GetAbstract?Abstract={id}");
            HttpResponseMessage result = await _client.PostRequest(item, "Abstract/DeleteAbstract");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Abstract Details have been deleted";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "An error occured";
                TempData["Type"] = "Error";
                TempData.Keep();
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Update(AbstractViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Abstract/UpdateAbstract");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Updated Abstract";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = vm.AbstractId });
            }
            else
            {
                return View(vm);
            }
        }
    }
}
