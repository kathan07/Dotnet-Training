using Final_POC.Core.DTOs;
using Final_POC.Web.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthServices _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthServices authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        } 

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login form");
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result)
            {
                _logger.LogInformation("User logged in successfully: {Email}", model.Email);
                return RedirectToAction("Privacy", "Home");
            }

            _logger.LogWarning("Failed login attempt for: {Email}", model.Email);
            ModelState.AddModelError("", "Invalid login attempt. Please check your email and password.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logging out");
            _authService.Logout();
            return RedirectToAction("Login");
        }
    }
}
