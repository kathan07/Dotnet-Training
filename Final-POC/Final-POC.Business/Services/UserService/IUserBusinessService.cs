using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;

namespace Final_POC.Business.Services.UserService
{
    public interface IUserBusinessService
    {
        Task<UserDto?> CreateUser(RegisterUserDto request);
        Task<bool> DeleteUser(int id);
        Task<UserDto?> EditUser(UpdateUserDto request);
        Task<UserDto?> GetUserById(int id);
        Task<List<UserDto>> GetUsers(Expression<Func<User, bool>>? predicate = null);

    }
}
