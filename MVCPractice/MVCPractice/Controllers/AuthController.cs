using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCPractice.Models;

namespace MVCPractice.Controllers
{
    public class AuthController : Controller
    {
        // GET: AuthController
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user); // Return the form with errors
            }

            return Content("User registered successfully!");
        }

       
    }
}
