using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EFCore.Data.Models;
using EFCore.Data.Repositories.CategoryRepository;
using EFCore.Service.DTOModels;

namespace EFCore.Service.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDTO?> AddCategory(CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                return null;
            }

            try
            {
                var category = _mapper.Map<Category>(categoryDTO);
                var response = await _categoryRepository.AddCategory(category);
                if (response == null) return null;
                return _mapper.Map<CategoryDTO>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategories();
                return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CategoryDTO?> UpdateCategory(CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                return null;
            }

            try
            {
                var existingCategory = await _categoryRepository.GetCategory(categoryDTO.Id);
                if (existingCategory == null)
                {
                    return null;
                }

                var category = _mapper.Map<Category>(categoryDTO);
                var response = await _categoryRepository.UpdateCategory(category);
                if (response == null) return null;
                return _mapper.Map<CategoryDTO>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategory(id);
                if (existingCategory == null)
                {
                    return false;
                }

                return await _categoryRepository.DeleteCategory(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

