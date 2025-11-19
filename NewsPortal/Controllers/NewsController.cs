using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using NewsPortal.Models;
using NewsPortal.Services;
using NewsPortal.ViewModels;

namespace NewsPortal.Controllers
{
    [Authorize]
    public class NewsController : BaseController
    {
        private readonly INewsService _newsService;
        private readonly IWebHostEnvironment _env;

        public NewsController(INewsService newsService, IWebHostEnvironment env)
        {
            _newsService = newsService;
            _env = env;
        }

        private string GetCurrentLanguage()
        {
            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            return feature?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "ru";
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            string lang = GetCurrentLanguage();
            var news = await _newsService.GetPagedNewsAsync(page, pageSize, lang);

            ViewBag.TotalCount = await _newsService.GetCountAsync();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_NewsListPartial", news);

            return View(news);
        }

        public IActionResult Create()
        {
            return View(new NewsCreateEditViewModel
            {
                Language = GetCurrentLanguage()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateEditViewModel model)
        {
            if (!TryValidateModel(model))
                return View(model);

            string lang = (model.Language ?? "ru").Trim().ToLowerInvariant();

            var news = new News
            {
                CreatedAt = DateTime.Now
            };

            if (model.ImageFile != null)
                news.ImagePath = await SaveNewsImageAsync(model.ImageFile);

            var translation = new NewsTranslation
            {
                Title = model.Title,
                Subtitle = model.Subtitle,
                Body = model.Body,
                Language = lang
            };

            await _newsService.CreateAsync(news, translation);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            string lang = GetCurrentLanguage();
            var news = await _newsService.GetByIdAsync(id);
            if (news == null)
                return NotFound();

            var translation = news.Translations.FirstOrDefault(t => t.Language == lang);

            return View(new NewsCreateEditViewModel
            {
                Id = news.Id,
                Title = translation?.Title ?? string.Empty,
                Subtitle = translation?.Subtitle ?? string.Empty,
                Body = translation?.Body ?? string.Empty,
                ImagePath = news.ImagePath,
                Language = lang
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsCreateEditViewModel model)
        {
            if (!TryValidateModel(model))
            {
                if (model.Id != null)
                {
                    var existingNews = await _newsService.GetByIdAsync(model.Id.Value);
                    model.ImagePath = existingNews?.ImagePath;
                }

                return View(model);
            }


            if (model.Id == null)
                return BadRequest();

            string lang = (model.Language ?? "ru").Trim().ToLowerInvariant();
            var news = await _newsService.GetByIdAsync(model.Id.Value);

            if (news == null)
                return NotFound();

            var translation = news.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation == null)
            {
                translation = new NewsTranslation
                {
                    NewsId = news.Id,
                    Language = lang
                };
                news.Translations.Add(translation);
            }

            translation.Title = model.Title;
            translation.Subtitle = model.Subtitle;
            translation.Body = model.Body;

            if (model.ImageFile != null)
                news.ImagePath = await SaveNewsImageAsync(model.ImageFile);

            await _newsService.UpdateAsync(news, translation);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _newsService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            string lang = GetCurrentLanguage();
            var news = await _newsService.GetByIdAsync(id);

            if (news == null)
                return NotFound();

            return View(news);
        }

        private async Task<string> SaveNewsImageAsync(IFormFile file)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/" + fileName;
        }
    }
}
