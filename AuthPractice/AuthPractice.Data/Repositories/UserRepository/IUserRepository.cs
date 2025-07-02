using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthPractice.Data.Models;

namespace AuthPractice.Data.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(User user);
    }
}
