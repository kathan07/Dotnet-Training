using System.Linq.Expressions;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;

namespace Final_POC.API.Services.UserServices
{
    public interface IUserApiService
    {
        Task<ApiResponseDto<UserDto>> RegisterUser(RegisterUserDto request);
        Task<ApiResponseDto<UserDto>> UpdateUser(UpdateUserDto request);
        Task<ApiResponseDto<bool>> DeleteUser(int id);
        Task<ApiResponseDto<UserDto>> GetUserById(int id);
        Task<ApiResponseDto<List<UserDto>>> GetUsers(Expression<Func<User, bool>>? predicate = null);
    }
}
