using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using EFCore.Data.Models;
using EFCore.Data.Repositories.ProductRepository;
using EFCore.Service.DTOModels;

namespace EFCore.Service.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductDTO?> AddProduct(ProductDTO product)
        {
            if (product == null)
            {
                return null;
            }

            try
            {
                var productEntity = _mapper.Map<Product>(product);
                var response = await _productRepository.AddProduct(productEntity);
                if (response == null) return null;
                var newProduct = await GetProduct(response.Id, true);
                return newProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductDTO?> GetProduct(int id, bool includeCategory = false)
        {
            try
            {
                var response = await _productRepository.GetProduct(id, includeCategory);
                if (response == null) return null;
                return _mapper.Map<ProductDTO>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts(
            Expression<Func<Product, bool>>? filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            bool includeCategory = false)
        {
            try
            {
                var response = await _productRepository.GetProducts(filter, orderBy, includeCategory);
                return response.Select(product => _mapper.Map<ProductDTO>(product));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductDTO?> UpdateProduct(ProductDTO product)
        {
            if (product == null)
            {
                return null;
            }

            try
            {
                var existingProduct = await _productRepository.GetProduct(product.Id);
                if (existingProduct == null)
                {
                    return null;
                }

                var productEntity = _mapper.Map<Product>(product);
                var response = await _productRepository.UpdateProduct(productEntity);
                if (response == null) return null;
                var updatedProduct = await GetProduct(response.Id, true);
                return updatedProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var existingProduct = await _productRepository.GetProduct(id);
                if (existingProduct == null)
                {
                    return false;
                }

                return await _productRepository.DeleteProduct(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
