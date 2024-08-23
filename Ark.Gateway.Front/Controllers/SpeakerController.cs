using Ark.Gateway.Front.Data.FileManager;
using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class SpeakerController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly IFileManager _fm;
        private readonly INotyfService _notifyService;
        private readonly string _imageurl;

        public SpeakerController(IHttpClientService client, INotyfService notifyService, IFileManager fm, IConfiguration configuration)
        {
            _client = client;
            _fm = fm;
            _notifyService = notifyService;
            _imageurl = configuration["ImageUrl"];
        }
        public async Task<IActionResult> Index()
        {

            List<SpeakerViewModel> result = await _client.GetRequest(new List<SpeakerViewModel>(), $"Speaker/GetSpeakers");
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
        public async Task<IActionResult> Create(SpeakerViewModel vm)
        {
            if (vm.TempImage is not null)
            {
                var imageName = _fm.SaveImage(vm.TempImage);
                vm.Image = imageName;
            }
            
            HttpResponseMessage result = await _client.PostRequest(vm, "Speaker/AddSpeaker");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Speaker";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(Guid Id)
        {
            SpeakerViewModel result = await _client.GetRequest(new SpeakerViewModel(), $"Speaker/GetSpeaker?SpeakerId={Id}");
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SpeakerViewModel vm)
        {
            SpeakerViewModel tmp = await _client.GetRequest(new SpeakerViewModel(), $"Speaker/GetSpeaker?SpeakerId={vm.SpeakerId}");
            vm.Image = tmp.Image;
            HttpResponseMessage result = await _client.PostRequest(vm, "Speaker/UpdateSpeaker");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Speaker";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Speaker(Guid id)
        {
            var result = await _client.GetRequest(new SpeakerViewModel(), $"Speaker/GetSpeaker?Speaker={id}");
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

            var result = await _client.GetRequest(new SpeakerViewModel(), $"Speaker/GetSpeaker?Speaker={id}");
            return View(result);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Speaker = new SpeakerViewModel { SpeakerId = id };
            HttpResponseMessage result = await _client.PostRequest(Speaker, "Speaker/DeleteSpeaker");
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

        public async Task<IActionResult> UpdateImage(Guid SpeakerId, IFormFile TempImage)
        {
            var res = await _client.GetRequest(new SpeakerViewModel(), $"Speaker/GetSpeaker?Speaker={SpeakerId}");

            if (TempImage is not null)
            {
                res.Image = _fm.SaveImage(TempImage);
            }

            HttpResponseMessage result = await _client.PostRequest(res, "Speaker/UpdateSpeaker");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = res.SpeakerId });
            }
            else
            {
                return RedirectToAction("Details", new { id = res.SpeakerId });

            }
        }

        public async Task<IActionResult> Update(SpeakerViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Speaker/UpdateSpeaker");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = vm.SpeakerId });
            }
            else
            {
                return View(vm);
            }
        }
    }
}
