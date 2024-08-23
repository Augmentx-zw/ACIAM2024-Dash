using Ark.Gateway.Front.Data.FileManager;
using Ark.Gateway.Front.Services;
using ArkPortal.Gateway.DTO.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Gateway.Front.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IHttpClientService _client;
        private readonly IFileManager _fm;
        private readonly INotyfService _notifyService;
        private readonly string _imageurl;

        public ArticleController(IHttpClientService client, INotyfService notifyService, IFileManager fm, IConfiguration configuration)
        {
            _client = client;
            _fm = fm;
            _notifyService = notifyService;
            _imageurl = configuration["ImageUrl"];
        }
        public async Task<IActionResult> Index()
        {

            List<ArticleViewModel> result = await _client.GetRequest(new List<ArticleViewModel>(), $"Article/GetArticles");
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
        public async Task<IActionResult> Create(ArticleViewModel vm)
        {
            if (vm.TempImage is not null)
            {
                var imageName = _fm.SaveImage(vm.TempImage);
                vm.Image = imageName;
            }
            
            HttpResponseMessage result = await _client.PostRequest(vm, "Article/AddArticle");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Article";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(Guid Id)
        {
            ArticleViewModel result = await _client.GetRequest(new ArticleViewModel(), $"Article/GetArticle?ArticleId={Id}");
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArticleViewModel vm)
        {
            ArticleViewModel tmp = await _client.GetRequest(new ArticleViewModel(), $"Article/GetArticle?ArticleId={vm.ArticleId}");
            vm.Image = tmp.Image;
            HttpResponseMessage result = await _client.PostRequest(vm, "Article/UpdateArticle");
            result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                TempData["Message"] = "Added new Article";
                TempData["Type"] = "Success";
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Article(Guid id)
        {
            var result = await _client.GetRequest(new ArticleViewModel(), $"Article/GetArticle?Article={id}");
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

            var result = await _client.GetRequest(new ArticleViewModel(), $"Article/GetArticle?Article={id}");
            return View(result);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Article = new ArticleViewModel { ArticleId = id };
            HttpResponseMessage result = await _client.PostRequest(Article, "Article/DeleteArticle");
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

        public async Task<IActionResult> UpdateImage(Guid ArticleId, IFormFile TempImage)
        {
            var res = await _client.GetRequest(new ArticleViewModel(), $"Article/GetArticle?Article={ArticleId}");

            if (TempImage is not null)
            {
                res.Image = _fm.SaveImage(TempImage);
            }

            HttpResponseMessage result = await _client.PostRequest(res, "Article/UpdateArticle");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = res.ArticleId });
            }
            else
            {
                return RedirectToAction("Details", new { id = res.ArticleId });

            }
        }

        public async Task<IActionResult> Update(ArticleViewModel vm)
        {
            HttpResponseMessage result = await _client.PostRequest(vm, "Article/UpdateArticle");
            ErrorCheck check = ValidationResponseCheck.IsValidResponse(result);
            if (!check.Error)
            {
                TempData["Message"] = "Success";
                TempData["Type"] = "Information";
                TempData.Keep();
                return RedirectToAction("Details", new { id = vm.ArticleId });
            }
            else
            {
                return View(vm);
            }
        }
    }
}
