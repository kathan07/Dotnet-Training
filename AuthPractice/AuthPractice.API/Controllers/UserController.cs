using System.Security.Claims;
using AuthPractice.API.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthPractice.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController: ControllerBase
    {
        private readonly IUserAPIService _userAPIService;
         
        public UserController(IUserAPIService userAPIService)
        {
            _userAPIService = userAPIService;
        }


        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            //foreach (var claim in User.Claims)
            //{
            //    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            //}
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                return Unauthorized();

            var user = await _userAPIService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "This is an admin-only endpoint" });
        }
    }
}
