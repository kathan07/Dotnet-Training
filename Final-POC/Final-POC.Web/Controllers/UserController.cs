using Final_POC.Core.DTOs;
using Final_POC.Web.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _userServices.GetAllUsersAsync();
                if (response.Success)
                {
                    return View(response.Data);
                }

                ViewBag.ErrorMessage = response.Message;
                return View(new List<UserDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                ViewBag.ErrorMessage = "Failed to fetch users.";
                return View(new List<UserDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _userServices.GetAllUsersAsync();
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                return StatusCode(500, new { success = false, message = "Failed to fetch users." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _userServices.GetUserByIdAsync(id);
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user details for ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Failed to retrieve user." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            try
            {
                var response = await _userServices.RegisterUserAsync(registerUserDto);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "User created successfully." });
                }

                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new user");
                return StatusCode(500, new { success = false, message = "An error occurred while creating the user." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _userServices.GetUserByIdAsync(id);
                if (response.Success)
                {
                    // Convert UserDto to UpdateUserDto for the edit form
                    var updateUserDto = new UpdateUserDto
                    {
                        Id = response.Data.Id,
                        Username = response.Data.Username,
                        Email = response.Data.Email,
                    };

                    return Ok(new { success = true, data = updateUserDto });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user for edit with ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Failed to retrieve user." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            try
            {
                var response = await _userServices.UpdateUserAsync(updateUserDto);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "User updated successfully." });
                }

                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", updateUserDto.Id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating the user." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _userServices.DeleteUserAsync(id);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "User deleted successfully." });
                }
                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return StatusCode(500, new { success = false, message = "Failed to delete user." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string username, string email)
        {
            try
            {
                var response = await _userServices.SearchUsersAsync(username, email);
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }

                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with username: {Username}, email: {Email}",
                    username ?? "null", email ?? "null");
                return StatusCode(500, new { success = false, message = "An error occurred while searching for users." });
            }
        }
    }
} 
