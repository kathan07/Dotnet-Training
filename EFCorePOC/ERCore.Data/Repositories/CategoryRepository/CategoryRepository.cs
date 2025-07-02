using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Contexts;
using EFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data.Repositories.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Category?> AddCategory(Category category)
        {
            if (category == null)
            {
                return null;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return category;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Category?> GetCategory(int id)
        {
            try
            {
                IQueryable<Category> query = _dbContext.Categories.AsNoTracking();
                return await query.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            try
            {
                IQueryable<Category> query = _dbContext.Categories.AsNoTracking();
                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Category?> UpdateCategory(Category category)
        {
            if (category == null)
            {
                return null;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return category;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var category = await _dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    return false;
                }

                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
