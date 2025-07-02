using Final_POC.Core.DTOs;
using Final_POC.Web.Services.ApiServices;

namespace Final_POC.Web.Services.UserServices
{
    public class UserServices: IUserServices
    {
        private readonly IApiServices _apiServices;
        private readonly ILogger<UserServices> _logger;

        public UserServices(IApiServices apiServices, ILogger<UserServices> logger)
        {
            _apiServices = apiServices;
            _logger = logger;
        }

        public async Task<ApiResponseDto<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                _logger.LogInformation("Attempting to register new user: {Username}", registerUserDto.Username);
                return await _apiServices.PostAsync<ApiResponseDto<UserDto>>("user/register", registerUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", registerUserDto.Username);
                return ApiResponseDto<UserDto>.FailResponse("An error occurred during registration");
            }
        }

        public async Task<ApiResponseDto<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with ID: {UserId}", updateUserDto.Id);
                return await _apiServices.PutAsync<ApiResponseDto<UserDto>>("user/updateuser", updateUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", updateUserDto.Id);
                return ApiResponseDto<UserDto>.FailResponse("An error occurred while updating user");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete user with ID: {UserId}", userId);
                return await _apiServices.DeleteAsync<ApiResponseDto<bool>>($"user/delete/{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
                return ApiResponseDto<bool>.FailResponse("An error occurred while deleting user");
            }
        }

        public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Retrieving user with ID: {UserId}", userId);
                return await _apiServices.GetAsync<ApiResponseDto<UserDto>>($"user/{userId}",null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                return ApiResponseDto<UserDto>.FailResponse("An error occurred while retrieving user");
            }
        }

        public async Task<ApiResponseDto<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all users");
                return await _apiServices.GetAsync<ApiResponseDto<List<UserDto>>>($"user/list/users", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return ApiResponseDto<List<UserDto>>.FailResponse("An error occurred while retrieving users");
            }
        }

        public async Task<ApiResponseDto<List<UserDto>>> SearchUsersAsync(string username = null, string email = null)
        {
            try
            {
                _logger.LogInformation("Searching users with username: {Username}, email: {Email}",
                    username ?? "null", email ?? "null");

                var queryParams = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(username))
                    queryParams.Add("username", username);

                if (!string.IsNullOrEmpty(email))
                    queryParams.Add("email", email);

                return await _apiServices.GetAsync<ApiResponseDto<List<UserDto>>>($"user/search", queryParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with username: {Username}, email: {Email}",
                    username ?? "null", email ?? "null");
                return ApiResponseDto<List<UserDto>>.FailResponse("An error occurred while searching users");
            }
        }


    }
}
