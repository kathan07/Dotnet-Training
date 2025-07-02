using Final_POC.Business.Services.AuthService;
using Final_POC.Core.DTOs;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.API.Services.AuthServices
{
    public class AuthApiService: IAuthApiService
    {
        private readonly IAuthBusinessService _authBusinessService;
        private readonly ILogger<AuthApiService> _logger;

        public AuthApiService(IAuthBusinessService authBusinessService, ILogger<AuthApiService> logger) 
        {
            _authBusinessService = authBusinessService;
            _logger = logger;
        }

        public async Task<ApiResponseDto<LoginReponseDto>> Login(LoginUserDto request)
        {
            _logger.LogInformation("Processing login request for email: {Email}", request.Email);

            var response = await _authBusinessService.Login(request);

            if (response == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return ApiResponseDto<LoginReponseDto>.FailResponse("Invalid credentials");
            }

            _logger.LogInformation("Login successful for email: {Email}", request.Email);
            return ApiResponseDto<LoginReponseDto>.SuccessResponse(response, "Login Successful");
        }


    }
}
