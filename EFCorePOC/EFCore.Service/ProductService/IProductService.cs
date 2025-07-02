using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;

namespace EFCore.Service.ProductService
{
    public interface IProductService
    {
        Task<ProductDTO?> AddProduct(ProductDTO product);
        Task<ProductDTO?> GetProduct(int id, bool includeCategory = false);
        Task<IEnumerable<ProductDTO>> GetProducts(
            Expression<Func<Product, bool>>? filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            bool includeCategory = false);
        Task<ProductDTO?> UpdateProduct(ProductDTO product);
        Task<bool> DeleteProduct(int id);
    }
}

