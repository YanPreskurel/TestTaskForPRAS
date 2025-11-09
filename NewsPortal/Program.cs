using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NewsPortal.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ƒобавл€ем сервисы локализации
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("AdminCookie")
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
    });

var app = builder.Build();

// Ќастройка локалей
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// QueryString провайдер первым, чтобы €зык мен€лс€ через ?culture=...
localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
// Cookie провайдер, чтобы выбранный €зык сохран€лс€
localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
