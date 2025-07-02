using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC_POC.Models;
using MVC_POC.Services.AuthService;

namespace MVC_POC.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
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
        [HttpPost]
        public IActionResult Signup(User user)
        {
            if (ModelState.IsValid)
            {
                if (_authService.RegisterUser(user)) {
                    var registeredUser = _authService.ValidateUser(user.Email, user.Password);
                    HttpContext.Session.SetString("User", JsonSerializer.Serialize(registeredUser));
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = "Email is already registered";
            }
            return View(user);
        }

        public IActionResult Signin()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Signin(LoginUser inputUser)
        {
            if (ModelState.IsValid)
            {
                var user = _authService.ValidateUser(inputUser.Email, inputUser.Password);
                if (user != null)
                {
                    string userJson = JsonSerializer.Serialize(user);
                    HttpContext.Session.SetString("User", userJson);
                    return RedirectToAction("Index", "Home");
                }

                TempData["error"] = "Invalid email or password";
            }
            return View(inputUser);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Signin");
        }

    }
}
