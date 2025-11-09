// HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;

public class HomeController : BaseController
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var latestNews = await _context.News
            .OrderByDescending(n => n.CreatedAt)
            .Take(3)
            .ToListAsync();

        return View(latestNews);
    }
}
