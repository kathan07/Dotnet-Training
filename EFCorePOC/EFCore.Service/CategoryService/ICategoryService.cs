using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;

namespace EFCore.Service.CategoryService
{
    public interface ICategoryService
    {
        Task<CategoryDTO?> AddCategory(CategoryDTO category);
        Task<IEnumerable<CategoryDTO>> GetAllCategories();
        Task<CategoryDTO?> UpdateCategory(CategoryDTO category);
        Task<bool> DeleteCategory(int id);
    }
}
