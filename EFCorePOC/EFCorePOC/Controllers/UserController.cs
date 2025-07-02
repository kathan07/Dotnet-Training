using System.Text.Json;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;
using EFCore.Service.UserService;
using EFCorePOC.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePOC.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IActionResult Signup()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user)) {
                return View();
            }

            return RedirectToAction("ProductList", "Product");
            
        }

        [HttpPost]
        public async Task<IActionResult> Signup(UserDTO userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Please complete all required fields correctly.";
                    return View(userDto);
                }

                var result = await _userService.SignUp(userDto);
                if (result == null)
                {
                    TempData["error"] = "Signup failed. An account with this email already exists.";
                    return View(userDto);
                }
                HttpContext.Session.SetString("User", JsonSerializer.Serialize(result));

                TempData["success"] = "Account created successfully!";
                return RedirectToAction("ProductList", "Product");
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred. Please try again later.";
                return View(userDto);
            }
        }

        public IActionResult Signin()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                return View();
            }

            return RedirectToAction("ProductList", "Product");
        }


        [HttpPost]
        public async Task<IActionResult> Signin(LoginUser loginUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Please enter your email and password correctly.";
                    return View(loginUser);
                }

                var result = await _userService.SignIn(loginUser.Email, loginUser.Password);
                if (result == null)
                {
                    TempData["error"] = "Invalid email or password.";
                    return View(loginUser);
                }

                //result = SanitizeUserForSession(result);
                HttpContext.Session.SetString("User", JsonSerializer.Serialize(result));

                return RedirectToAction("ProductList", "Product"); // Returning the UserDTO object as JSON
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred. Please try again later.";
                return View(loginUser);
            }
        }

        [HttpPost]
        public IActionResult Signout()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Signin", "User");
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred during sign out.";
                return RedirectToAction("Signin", "User");
            }
        }
    }
}
