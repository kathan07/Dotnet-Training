using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;

namespace EFCore.Service.UserService
{
    public interface IUserService
    {
        Task<UserDTO?> SignUp(UserDTO userDto);
        Task<UserDTO?> SignIn(string email, string password);
    }
}
