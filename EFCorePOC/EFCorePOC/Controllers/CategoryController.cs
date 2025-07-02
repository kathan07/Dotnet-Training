using EFCore.Data.Models;
using EFCore.Service.CategoryService;
using Microsoft.AspNetCore.Mvc;
using EFCorePOC.Web.Models;
using EFCore.Service.DTOModels;

namespace EFCorePOC.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _categoryService.AddCategory(category);
                if (result == null)
                {
                    return StatusCode(500, "Failed to add the category");
                }

                TempData["message"] = $"{result.Name} has been added successfully.";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditCategory([FromBody] CategoryDTO category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _categoryService.UpdateCategory(category);
                if (result == null)
                {
                    return NotFound("Category not found.");
                }

                TempData["message"] = $"{result.Name} has been updated successfully.";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategory(id);
                if (!success)
                {
                    return NotFound("Category not found or could not be deleted.");
                }

                TempData["message"] = "Category deleted successfully.";
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> ListCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCategory([FromQuery] string id)
        {
            try
            {
                int idx = int.Parse(id);
                if (idx <= 0)
                {
                    return BadRequest("Invalid category ID");
                }

                var categories = await _categoryService.GetAllCategories();
                var category = categories.FirstOrDefault(category => category.Id == idx);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
