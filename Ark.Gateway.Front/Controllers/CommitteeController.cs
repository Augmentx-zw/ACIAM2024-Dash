using Ark.Gateway.Front.Data.FileManager;
using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class CommitteeController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly IFileManager _fm;
        private readonly INotyfService _notifyService;
        private readonly string _imageurl;

        public CommitteeController(IHttpClientService client, INotyfService notifyService, IFileManager fm, IConfiguration configuration)
        {
            _client = client;
            _fm = fm;
            _notifyService = notifyService;
            _imageurl = configuration["ImageUrl"];
        }
        public async Task<IActionResult> Index()
        {

            List<CommitteeViewModel> result = await _client.GetRequest(new List<CommitteeViewModel>(), $"Committee/GetCommittees");
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
            ViewBag.Url = _imageurl;
            return View(result.OrderByDescending(s => s.CreatedOn));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommitteeViewModel vm)
        {
            if (vm.TempImage is not null)
            {
                var imageName = _fm.SaveImage(vm.TempImage);
                vm.Image = imageName;
            }
            
            HttpResponseMessage result = await _client.PostRequest(vm, "Committee/AddCommittee");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Committee";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        public async Task<IActionResult> Committee(Guid id)
        {
            var result = await _client.GetRequest(new CommitteeViewModel(), $"Committee/GetCommittee?Committee={id}");
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
            ViewBag.Url = _imageurl;

            var result = await _client.GetRequest(new CommitteeViewModel(), $"Committee/GetCommittee?Committee={id}");
            return View(result);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Committee = new CommitteeViewModel { CommitteeId = id };
            HttpResponseMessage result = await _client.PostRequest(Committee, "Committee/DeleteCommittee");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> UpdateImage(Guid CommitteeId, IFormFile TempImage)
        {
            var res = await _client.GetRequest(new CommitteeViewModel(), $"Committee/GetCommittee?Committee={CommitteeId}");

            if (TempImage is not null)
            {
                res.Image = _fm.SaveImage(TempImage);
            }

            HttpResponseMessage result = await _client.PostRequest(res, "Committee/UpdateCommittee");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = res.CommitteeId });
            }
            else
            {
                return RedirectToAction("Details", new { id = res.CommitteeId });

            }
        }

        public async Task<IActionResult> Update(CommitteeViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Committee/UpdateCommittee");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = vm.CommitteeId });
            }
            else
            {
                return View(vm);
            }
        }
    }
}
