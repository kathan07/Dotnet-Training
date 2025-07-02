using AuthPractice.Business.Services.UserService;
using AuthPractice.Core.DTOs;

namespace AuthPractice.API.Services.UserServices
{
    public class UserAPIService: IUserAPIService
    {
        private readonly IUserBusinessService _userService;

        public UserAPIService(IUserBusinessService userService)
        {
            _userService = userService;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _userService.GetUserByIdAsync(id);
        }
    }
}
