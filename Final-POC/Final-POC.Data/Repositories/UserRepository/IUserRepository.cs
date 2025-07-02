using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Data.Models;

namespace Final_POC.Data.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUser(Expression<Func<User, bool>> predicate);
        Task<List<User>> GetUsers(Expression<Func<User, bool>>? predicate = null);
        Task<User?> CreateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<User?> UpdateUser(User user);
    }
}
