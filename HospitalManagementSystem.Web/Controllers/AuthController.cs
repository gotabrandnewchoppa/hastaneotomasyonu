using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagementSystem.Web.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Zaten giriş yapıldıysa ana sayfaya yönlendir
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var adminEmail = _configuration["AdminCredentials:Email"];
            var adminPassword = _configuration["AdminCredentials:Password"];
            var adminName = _configuration["AdminCredentials:Name"] ?? "Yönetici";

            if (email == adminEmail && password == adminPassword)
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserName", adminName);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        // GET: Auth/Logout — Tarayıcı adres çubuğundan direkt girilirse Login'e yönlendir
        [HttpGet]
        [ActionName("Logout")]
        public IActionResult LogoutGet()
        {
            return RedirectToAction("Login");
        }

        // POST: Auth/Logout — Sidebar formu üzerinden güvenli çıkış
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public IActionResult LogoutPost()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
