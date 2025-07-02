using AuthPractice.Core.DTOs;

namespace AuthPractice.API.Services.UserServices
{
    public interface IUserAPIService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}
