using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Repositories;
using NewsPortal.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Локализация
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<ITranslationService, GoogleTranslationService>();

// Auth
builder.Services.AddAuthentication("AdminCookie")
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
    });

var app = builder.Build();

// НАСТРОЙКА КУЛЬТУР — ПРАВИЛЬНО
var supportedCultures = new[] { "ru", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("ru")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Применение локализации
app.UseRequestLocalization(localizationOptions);

// Middlewares
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Создание админа
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.AdminUsers.Any())
    {
        db.AdminUsers.Add(new NewsPortal.Models.AdminUser
        {
            Email = "admin@example.com",
            Name = "Super Admin",
            PasswordHash = ComputeSha256Hash("Admin@123")
        });
        db.SaveChanges();
    }
}

static string ComputeSha256Hash(string rawData)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
    var sb = new System.Text.StringBuilder();
    foreach (var b in bytes)
        sb.Append(b.ToString("x2"));
    return sb.ToString();
}

app.Run();
