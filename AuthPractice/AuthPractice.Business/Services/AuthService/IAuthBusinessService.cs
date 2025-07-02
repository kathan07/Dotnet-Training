using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthPractice.Core.DTOs;

namespace AuthPractice.Business.Services.AuthService
{
    public interface IAuthBusinessService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
