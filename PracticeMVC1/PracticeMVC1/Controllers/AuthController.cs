using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for session
using PracticeMVC1.Models;
using PracticeMVC1.Services;
using PracticeMVC1.Services.CredentialService;

namespace PracticeMVC1.Controllers
{
    public class AuthController : Controller
    {
        private readonly CredentialService _credentialService;

        public AuthController(CredentialService credentialService)
        {
            _credentialService = credentialService;
        }

        public IActionResult Signin()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) // Fixed extra closing parenthesis
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Signin(User user)
        {
            if (_credentialService.credentials.TryGetValue(user.Username, out string storedPassword) && storedPassword == user.Password)
            {
                HttpContext.Session.SetString("User", user.Username); // Store user in session
                string role = user.Username == "admin" ? "admin" : "member";
                HttpContext.Session.SetString("Role", role);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Invalid Credentials!";
            return View();
        }

        public IActionResult Signup()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user)
        {
            if (!_credentialService.credentials.ContainsKey(user.Username))
            {   
                _credentialService.credentials[user.Username] = user.Password;
            }
            return RedirectToAction("Signin");
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Signin");
        }
    }
}
