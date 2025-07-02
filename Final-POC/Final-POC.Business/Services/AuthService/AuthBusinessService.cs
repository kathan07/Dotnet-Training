using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Final_POC.Core.DTOs;
using Final_POC.Data.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace Final_POC.Business.Services.AuthService
{
    public class AuthBusinessService: IAuthBusinessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthBusinessService> _logger;

        public AuthBusinessService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, ILogger<AuthBusinessService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginReponseDto?> Login(LoginUserDto request)
        {
            try
            {
                var user = await _userRepository.GetUser(x => x.Email == request.Email);
                if (user == null || !VerifyPasswordHash(request.Password, user.Password))
                {
                    _logger.LogWarning("Login failed for email: {Email}", request.Email);
                    return null;
                }

                var userDto = _mapper.Map<UserDto>(user);
                var token = GenerateJwtToken(user.Id, user.Username, user.Email, user.Role);

                return new LoginReponseDto
                {
                    Token = token,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user: {Email}", request.Email);
                return null;
            }
        }

        private string GenerateJwtToken(int userId, string username, string email, string role)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate JWT token for userId: {UserId}", userId);
                throw; // rethrow because token generation failure is critical
            }
        }
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

    }
}
