using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PAW3.Data.Models;    
using PAW3.Data.Repositories; 

namespace PAW3.Core.BusinessLogic
{
    public interface IProductBusiness
    {
        Task<bool> DeleteProductAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetProducts(int? id, CancellationToken ct = default);
        Task<bool> SaveProductAsync(Product product, CancellationToken ct = default);
    }

    public class ProductBusiness : IProductBusiness
    {
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IUnitOfWork _uow;

        public ProductBusiness(IRepositoryProduct repositoryProduct, IUnitOfWork uow)
        {
            _repositoryProduct = repositoryProduct;
            _uow = uow;
        }

        public async Task<bool> SaveProductAsync(Product product, CancellationToken ct = default)
        {
            // Si productId != 0 busca si es create o update
            var entity = product.ProductId != 0
                ? await _repositoryProduct.FindAsync(product.ProductId, ct)
                : null;

            if (entity is null)
            {
                // CREATE
                product.LastModified = DateTime.UtcNow;
                await _repositoryProduct.AddAsync(product, ct);
            }
            else
            {
                // UPDATE
                entity.ProductName = product.ProductName;
                entity.InventoryId = product.InventoryId;
                entity.SupplierId = product.SupplierId;
                entity.Description = product.Description;
                entity.Rating = product.Rating;
                entity.CategoryId = product.CategoryId;
                entity.ModifiedBy = product.ModifiedBy;
                entity.LastModified = DateTime.UtcNow;

                _repositoryProduct.Update(entity);
            }

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repositoryProduct.FindAsync(id, ct);
            if (entity is null) return false;

            _repositoryProduct.Remove(entity);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<Product>> GetProducts(int? id, CancellationToken ct = default)
        {
            if (id is null)
            {
                // Lista completa
                return await _repositoryProduct.ReadAsync(null, ct);
            }

            var one = await _repositoryProduct.FindAsync(id.Value, ct);
            return one is null ? Enumerable.Empty<Product>() : new[] { one };
        }
    }
}