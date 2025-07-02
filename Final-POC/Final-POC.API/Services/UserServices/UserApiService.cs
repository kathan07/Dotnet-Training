using System.Linq.Expressions;
using Final_POC.Business.Services.UserService;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;
using Microsoft.Extensions.Logging;

namespace Final_POC.API.Services.UserServices
{
    public class UserApiService: IUserApiService
    {
        private readonly IUserBusinessService _userBusinessService;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(IUserBusinessService userBusinessService, ILogger<UserApiService> logger)
        {
            _userBusinessService = userBusinessService;
            _logger = logger;
        }

        public async Task<ApiResponseDto<UserDto>> RegisterUser(RegisterUserDto request)
        {
            _logger.LogInformation("Processing user registration request for {Email}", request.Email);

            var result = await _userBusinessService.CreateUser(request);

            if (result == null)
            {
                _logger.LogWarning("User registration failed for {Email} - likely duplicate email or username", request.Email);
                return ApiResponseDto<UserDto>.FailResponse("User registration failed.");
            }

            _logger.LogInformation("User registration successful for {Email} with ID {UserId}", request.Email, result.Id);
            return ApiResponseDto<UserDto>.SuccessResponse(result, "User registered successfully.");
        }

        public async Task<ApiResponseDto<UserDto>> UpdateUser(UpdateUserDto request)
        {
            _logger.LogInformation("Processing user update request for ID {UserId}", request.Id);

            var result = await _userBusinessService.EditUser(request);

            if (result == null)
            {
                _logger.LogWarning("User update failed for ID {UserId} - user not found or duplicate email/username", request.Id);
                return ApiResponseDto<UserDto>.FailResponse("User update failed.");
            }

            _logger.LogInformation("User update successful for ID {UserId}", result.Id);
            return ApiResponseDto<UserDto>.SuccessResponse(result, "User updated successfully.");
        }

        public async Task<ApiResponseDto<bool>> DeleteUser(int id)
        {
            _logger.LogInformation("Processing user deletion request for ID {UserId}", id);

            var result = await _userBusinessService.DeleteUser(id);

            if (!result)
            {
                _logger.LogWarning("User deletion failed for ID {UserId}", id);
                return ApiResponseDto<bool>.FailResponse("User deletion failed.");
            }

            _logger.LogInformation("User deletion successful for ID {UserId}", id);
            return ApiResponseDto<bool>.SuccessResponse(true, "User deleted successfully.");
        }

        public async Task<ApiResponseDto<UserDto>> GetUserById(int id)
        {
            _logger.LogInformation("Processing request to get user with ID {UserId}", id);
            var result = await _userBusinessService.GetUserById(id);
            if (result == null)
            {
                _logger.LogWarning("User not found with ID {UserId}", id);
                return ApiResponseDto<UserDto>.FailResponse("User not found.");
            }
            _logger.LogInformation("Successfully retrieved user with ID {UserId}", id);
            return ApiResponseDto<UserDto>.SuccessResponse(result, "User retrieved successfully.");
        }

        public async Task<ApiResponseDto<List<UserDto>>> GetUsers(Expression<Func<User, bool>>? predicate = null)
        {
            _logger.LogInformation("Processing request to get users with condition");
            var results = await _userBusinessService.GetUsers(predicate);
            if (results == null || !results.Any())
            {
                _logger.LogInformation("No users found matching the specified condition");
                return ApiResponseDto<List<UserDto>>.SuccessResponse(new List<UserDto>(), "No users found.");
            }
            _logger.LogInformation("Successfully retrieved {Count} users", results.Count);
            return ApiResponseDto<List<UserDto>>.SuccessResponse(results, "Users retrieved successfully.");
        }


    }
}
