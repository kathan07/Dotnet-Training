using Final_POC.Core.DTOs;

namespace Final_POC.Web.Services.UserServices
{
    public interface IUserServices
    {
        Task<ApiResponseDto<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<ApiResponseDto<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<ApiResponseDto<bool>> DeleteUserAsync(int userId);
        Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId);
        Task<ApiResponseDto<List<UserDto>>> GetAllUsersAsync();
        Task<ApiResponseDto<List<UserDto>>> SearchUsersAsync(string username = null, string email = null);
    }
}
