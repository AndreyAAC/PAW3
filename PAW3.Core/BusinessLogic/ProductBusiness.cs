using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PAW3.Data.Models;
using PAW3.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAW3.Core.BusinessLogic;
//anmdrey asdjioejhtqewh
public interface IProductBusiness
{
    /// <summary>
    /// Deletes the product associated with the product id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteProductAsync(int id );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<Product>> GetProducts(int? id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    Task<bool> SaveProductAsync(Product product);
}

public class ProductBusiness(IRepositoryProduct repositoryProduct) : IProductBusiness
{
    /// </inheritdoc>
    public async Task<bool> SaveProductAsync(Product product)
    {
        var entity = await repositoryProduct.FindAsync(product.ProductId);

        if (entity == null)
        {
            return await repositoryProduct.CreateAsync(product);
        }
        else
        {
            entity.ProductName = product.ProductName;
            entity.InventoryId = product.InventoryId;
            entity.SupplierId = product.SupplierId;
            entity.Description = product.Description;
            entity.Rating = product.Rating;
            entity.CategoryId = product.CategoryId;
            entity.LastModified = product.LastModified ?? DateTime.UtcNow;
            entity.ModifiedBy = product.ModifiedBy;

            return await repositoryProduct.UpdateAsync(entity);
        }
    }

    /// </inheritdoc>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var entity = await repositoryProduct.FindAsync(id);
        return entity is null ? false : await repositoryProduct.DeleteAsync(entity);
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Product>> GetProducts(int? id)
    {
        return id == null
            ? await repositoryProduct.ReadAsync()
            : [await repositoryProduct.FindAsync((int)id)];
    }
}
