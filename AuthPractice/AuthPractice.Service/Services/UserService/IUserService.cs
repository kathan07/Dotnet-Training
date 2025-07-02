using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthPractice.Core.DTOs;

namespace AuthPractice.Service.Services.UserService
{
    public interface IUserService
    {
        Task<UserDto?> GetProfileAsync(string token);
    }
}
