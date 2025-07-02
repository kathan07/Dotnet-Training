using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;

namespace EFCore.Data.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<Product?> AddProduct(Product product);
        Task<Product?> GetProduct(int id, bool includeCategory = false);
        Task<IEnumerable<Product>> GetProducts(
            Expression<Func<Product, bool>>? filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            bool includeCategory = false);
        Task<Product?> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
    }
}
