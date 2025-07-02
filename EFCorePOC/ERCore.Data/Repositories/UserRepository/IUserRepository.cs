using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;

namespace EFCore.Data.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> AddUser(User user);
        Task<User?> GetUser(String Email);
    }
}
