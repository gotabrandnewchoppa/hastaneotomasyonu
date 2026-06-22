using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        // Demo admin bilgileri (gerçek projede Identity veya DB kullanılır)
        private const string AdminEmail = "admin@medipanel.com";
        private const string AdminPassword = "123456";
        private const string AdminName = "Yönetici";

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
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (email == AdminEmail && password == AdminPassword)
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserName", AdminName);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
