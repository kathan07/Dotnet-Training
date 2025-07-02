using System.Globalization;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;
using EFCore.Service.ProductService;
using EFCorePOC.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePOC.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _productService.AddProduct(product);
                if (result == null)
                {
                    return StatusCode(500, "Failed to add the product");
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
        public async Task<IActionResult> EditProduct([FromBody] ProductDTO product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productService.UpdateProduct(product);
                if (result == null)
                {
                    return NotFound("Product not found.");
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
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProduct(id);
                if (!success)
                {
                    return NotFound("Product not found or could not be deleted.");
                }

                TempData["message"] = "Product deleted successfully.";
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> ProductList()
        {
            try
            {
                var products = await _productService.GetProducts(null, null, includeCategory: true);

                return View(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        
        public async Task<IActionResult> ListProducts(
            [FromQuery] int? id = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                var products = await _productService.GetProducts(
                    filter: p => (!id.HasValue || p.Id == id.Value) &&
                                 (string.IsNullOrEmpty(search) || p.Name.Contains(search)) &&
                                 (!categoryId.HasValue || p.CategoryId == categoryId) &&
                                 (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                                 (!maxPrice.HasValue || p.Price <= maxPrice.Value),
                    orderBy: sortBy switch
                    {
                        "name" => q => q.OrderBy(p => p.Name),
                        "price" => q => q.OrderBy(p => p.Price),
                        "price_desc" => q => q.OrderByDescending(p => p.Price),
                        "name_desc" => q => q.OrderByDescending(p => p.Name),
                        _ => null
                    },
                    includeCategory: true
                );

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
