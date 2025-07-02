using Final_POC.Core.DTOs;

namespace Final_POC.API.Services.AuthServices
{
    public interface IAuthApiService
    {
        Task<ApiResponseDto<LoginReponseDto>> Login(LoginUserDto request);
    }
}
