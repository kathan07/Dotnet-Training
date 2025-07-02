using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Data.Contexts;
using Final_POC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Final_POC.Data.Repositories.UserRepository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetUser(Expression<Func<User, bool>> predicate)
        {
            try
            {
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user with predicate: {Predicate}", predicate);
                return null;
            }
        }

        public async Task<List<User>> GetUsers(Expression<Func<User, bool>>? predicate = null)
        {
            try
            {
                var query = _context.Users.AsNoTracking();
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users");
                return new List<User>();
            }
        }

        public async Task<User?> CreateUser(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create user: {@User}", user);
                return null;
            }
        }

        public async Task<bool> DeleteUser(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to delete user with ID: {UserId}", user.Id);
                return false;
            }
        }

        public async Task<User?> UpdateUser(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (existingUser == null) return null;

                existingUser.Username = user.Username;
                existingUser.Email = user.Email;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to update user with ID: {UserId}", user.Id);
                return null;
            }
        }
    }
}