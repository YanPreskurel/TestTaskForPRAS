using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Models;
using NewsPortal.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewsPortal.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAdminUserService _userService;

        public AccountController(IAdminUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Введите email и пароль";
                return View();
            }

            var user = await _userService.AuthenticateAsync(email, password);

            if (user == null)
            {
                ViewBag.Error = "Неверный email или пароль";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "AdminCookie");
            await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "News");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login");
        }
    }
}
