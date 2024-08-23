using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly INotyfService _notifyService;
        private readonly string _imageurl;

        public PaymentController(IHttpClientService client, INotyfService notifyService, IConfiguration configuration)
        {
            _client = client;
            _notifyService = notifyService;
            _imageurl = configuration["ImageUrl"];
        }

        public async Task<IActionResult> Index()
        {
            List<PaymentViewModel> result = await _client.GetRequest(new List<PaymentViewModel>(), $"paymentv1/GetAllPayments");


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
            var resToView = result.OrderBy(s => s.CreatedOn.DayOfYear).Where(p => p.Status == "SUCCESS" || p.Status == "Pay On Delivery").ToList();
            return View(resToView);
        }

        public async Task<IActionResult> Payment(string payment)
        {
            var pay_id = Guid.Parse(payment);
            PaymentViewModel result = await _client.GetRequest(new PaymentViewModel(), $"Payment/GetPayment?payment={pay_id}");
            return View(result);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var Payment = new PaymentViewModel
            {
                PaymentId = id
            };
            HttpResponseMessage result = await _client.PostRequest(Payment, "Paymentv1/DeletePayment");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Payment Details have been deleted";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Update(PaymentViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "PaymePaymentv1nt/UpdatePayment");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Updated Payment";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }
        }

    }
}
