using System.Linq.Expressions;
using Final_POC.API.Services.UserServices;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.API.Controllers.v1
{
    [Route("v1/api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApiService _userApiService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserApiService userApiService, ILogger<UserController> logger)
        {
            _userApiService = userApiService;
            _logger = logger;
        }


        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user registration");
                return BadRequest(ApiResponseDto<UserDto>.FailResponse("Invalid user registration data"));
            }

            var response = await _userApiService.RegisterUser(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPut("updateuser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user update");
                return BadRequest(ApiResponseDto<UserDto>.FailResponse("Invalid user update data"));
            }

            var response = await _userApiService.UpdateUser(request);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userApiService.DeleteUser(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userApiService.GetUserById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("list/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userApiService.GetUsers();
            return Ok(response);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? username = null, [FromQuery] string? email = null)
        {
            Expression<Func<User, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
            {
                predicate = user => user.Username.Contains(username) && user.Email.Contains(email);
            }
            else if (!string.IsNullOrEmpty(username))
            {
                predicate = user => user.Username.Contains(username);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                predicate = user => user.Email.Contains(email);
            }

            var response = await _userApiService.GetUsers(predicate);
            return Ok(response);
        }
    }
}
