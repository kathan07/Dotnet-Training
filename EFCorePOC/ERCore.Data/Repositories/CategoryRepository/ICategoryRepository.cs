using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;

namespace EFCore.Data.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<Category?> AddCategory(Category category);
        Task<Category?> GetCategory(int id);
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int id);
    }
}
