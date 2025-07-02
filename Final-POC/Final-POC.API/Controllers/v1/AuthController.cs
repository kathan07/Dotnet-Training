using Final_POC.API.Services.AuthServices;
using Final_POC.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.API.Controllers.v1
{
    [Route("v1/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthApiService authApiService, ILogger<AuthController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request");
                return BadRequest(ApiResponseDto<LoginReponseDto>.FailResponse("Invalid login data"));
            }

            var result = await _authApiService.Login(request);

            if (!result.Success)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized(result);
            }

            _logger.LogInformation("Successful login for email: {Email}", request.Email);
            return Ok(result);
        }

    }
}
