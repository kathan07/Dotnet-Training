using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_POC.Models;
using System.Text.Json;
using MVC_POC.Services.AuthService;

namespace MVC_POC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthService _authService;

        public HomeController(ILogger<HomeController> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public IActionResult Index(string? countryFilter)
        {
            var currentUserJson = HttpContext.Session.GetString("User");
            var currentUser = JsonSerializer.Deserialize<User>(currentUserJson);
            ViewBag.IsAdmin = currentUser.Role == RoleType.Admin;
            ViewBag.CurrentUserEmail = currentUser.Email;

            if (currentUser.Role == RoleType.Admin)
            {
                ViewBag.Countries = _authService.credentials.Values.Select(u => u.Country).Distinct();
            }

            return View();
        }

        public IActionResult GetUsers(string? countryFilter)
        {
            var currentUserJson = HttpContext.Session.GetString("User");
            var currentUser = JsonSerializer.Deserialize<User>(currentUserJson);
            IEnumerable<UserViewModel> users = _authService.credentials.Values
            .Where(u => currentUser.Role == RoleType.Member ? u.Country == currentUser.Country : true)
            .Where(u => string.IsNullOrEmpty(countryFilter) || u.Country == countryFilter)
            .Where(u => currentUser.Role == RoleType.Member ? u.Role != RoleType.Admin : true)
            .Where(u=> currentUser.Email!=u.Email)
            .Select((u, index) => new UserViewModel
            {
                SrNo = index + 1,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            })
            .ToList();
            return PartialView("UserList", users);
        }

        public IActionResult GetUserDetails(string email, string countryFilter)
        {
            var user = _authService.credentials.Values.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();
            var currentUserJson = HttpContext.Session.GetString("User");
            var currentUser = JsonSerializer.Deserialize<User>(currentUserJson);
            ViewBag.IsAdmin = currentUser.Role == RoleType.Admin;
            ViewBag.CurrentUserEmail = currentUser.Email;
            ViewBag.CountryFilter = countryFilter;
            return PartialView("UserDetails", user);
        }

        [HttpPost]
        public IActionResult UpdateUser([FromBody] User updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest("Invalid user data.");
            }
            if (!_authService.credentials.ContainsKey(updatedUser.Email))
            {
                return NotFound("User not found.");
            }
            var currentUserJson = HttpContext.Session.GetString("User");
            var currentUser = JsonSerializer.Deserialize<User>(currentUserJson);
            var existingUser = _authService.credentials[updatedUser.Email];
            if (currentUser.Role == RoleType.Member && currentUser.Email != updatedUser.Email)
            {
                return Unauthorized("Members can only update their own details.");
            }
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Country = updatedUser.Country;
            existingUser.State = updatedUser.State;
            existingUser.Phone = updatedUser.Phone;
            if (currentUser.Role == RoleType.Admin)
            {
                existingUser.Role = updatedUser.Role;
            }
            return Ok(new { message = "User updated successfully!" });
        }

        [HttpPost]
        public IActionResult DeleteUser(string email)
        {
            var currentUserJson = HttpContext.Session.GetString("User");
            var currentUser = JsonSerializer.Deserialize<User>(currentUserJson);
            if (currentUser.Role != RoleType.Admin || !_authService.credentials.ContainsKey(email))
            {
                return Unauthorized();
            }
            _authService.credentials.Remove(email);
            return Ok();
        }


        [HttpPost]
        public IActionResult UpdateProfile([FromBody] User updatedUser)
        {
            var sessionUserJson = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(sessionUserJson))
            {
                return Unauthorized(new { message = "User not logged in." });
            }
            var currentUser = JsonSerializer.Deserialize<User>(sessionUserJson);
            if (currentUser.Email == "admin@example.com")
            {
                return Forbid();
            }
            if (!_authService.credentials.ContainsKey(updatedUser.Email))
            {
                return NotFound(new { message = "User not found." });
            }
            var existingUser = _authService.credentials[updatedUser.Email];
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Country = updatedUser.Country;
            existingUser.State = updatedUser.State;
            HttpContext.Session.SetString("User", JsonSerializer.Serialize(existingUser));
            return Ok(new { message = "Profile updated successfully!" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
