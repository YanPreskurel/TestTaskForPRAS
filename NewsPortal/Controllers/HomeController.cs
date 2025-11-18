using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using NewsPortal.Repositories;

public class HomeController : BaseController
{
    private readonly INewsRepository _newsRepository;

    public HomeController(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<IActionResult> Index()
    {
        var feature = HttpContext.Features.Get<IRequestCultureFeature>();
        string language = feature?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "ru";

        var latestNews = await _newsRepository.GetLatestAsync(3, language);
        return View(latestNews);
    }
}
