using AuthPractice.API.Services.AuthServices;
using AuthPractice.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AuthPractice.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthAPIService _authAPIService;

        public AuthController(IAuthAPIService authAPIService)
        {
            _authAPIService = authAPIService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authAPIService.AuthenticateAsync(request);
            if (response == null)
                return Unauthorized(new { message = "Username or password is incorrect" });
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authAPIService.RegisterAsync(request);
            if (!result)
                return BadRequest(new { message = "Registration failed" });

            return Ok(new { message = "Registration successful" });
        }
    }
}
