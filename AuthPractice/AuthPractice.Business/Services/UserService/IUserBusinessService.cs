using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthPractice.Core.DTOs;

namespace AuthPractice.Business.Services.UserService
{
    public interface IUserBusinessService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}
