using Final_POC.Core.DTOs;

namespace Final_POC.Web.Services.AuthServices
{
    public interface IAuthServices
    {
        Task<bool> LoginAsync(LoginUserDto loginRequest);
        void Logout();
    }
}
