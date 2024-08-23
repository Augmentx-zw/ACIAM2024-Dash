using Ark.Gateway.Front.Data.FileManager;
using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly IFileManager _fm;
        private readonly INotyfService _notifyService;
        private readonly string _imageurl;

        public RegistrationController(IHttpClientService client, INotyfService notifyService, IFileManager fm, IConfiguration configuration)
        {
            _client = client;
            _fm = fm;
            _notifyService = notifyService;
            _imageurl = configuration["ImagePosts"];
        }
        public async Task<IActionResult> Index()
        {

            List<RegistrationViewModel> result = await _client.GetRequest(new List<RegistrationViewModel>(), $"Registration/GetRegistrations");
            List<PaymentViewModel> payResult = await _client.GetRequest(new List<PaymentViewModel>(), $"payment/GetAllPayments");

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
            var processedResult = new List<RegistrationViewModel>();

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].RegistrationStatus == "Self-funded")
                {
                    if (payResult.Where(p => p.RegistrationId == result[i].RegistrationId).FirstOrDefault()?.Status == "Paid")
                    {
                        processedResult.Add(result[i]);
                    }
                }
                else if (result[i].RegistrationStatus == "Sponsorship")
                {
                    processedResult.Add(result[i]);

                }
            }

            // send and email with the confimation email else click this link to process payemnt 
            // hook pay now
            // send an email with the calender event 

            TempData?.Remove("Message");
            TempData?.Remove("Type");
            ViewBag.Url = _imageurl;
            return View(processedResult.OrderByDescending(s => s.CreatedOn));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegistrationViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Registration/AddRegistration");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Registration";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        public async Task<IActionResult> Registration(Guid id)
        {
            var result = await _client.GetRequest(new RegistrationViewModel(), $"Registration/GetRegistration?Registration={id}");
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

            var result = await _client.GetRequest(new RegistrationViewModel(), $"Registration/GetRegistration?Registration={id}");
            return View(result);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Registration = new RegistrationViewModel { RegistrationId = id };
            HttpResponseMessage result = await _client.PostRequest(Registration, "Registration/DeleteRegistration");
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

        public async Task<IActionResult> Update(RegistrationViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Registration/UpdateRegistration");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = vm.RegistrationId });
            }
            else
            {
                return View(vm);
            }
        }
    }
}
