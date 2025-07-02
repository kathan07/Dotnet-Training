using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Core.DTOs;

namespace Final_POC.Business.Services.AuthService
{
    public interface IAuthBusinessService
    {
        Task<LoginReponseDto?> Login(LoginUserDto request);
    }
}
