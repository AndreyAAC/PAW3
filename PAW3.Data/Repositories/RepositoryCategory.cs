using Microsoft.EntityFrameworkCore;
using PAW3.Data.Models;

namespace PAW3.Data.Repositories;

public interface IRepositoryCategory
{
    Task<bool> UpsertAsync(Category entity, bool isUpdating);
    Task<bool> CreateAsync(Category entity);
    Task<bool> DeleteAsync(Category entity);
    Task<IEnumerable<Category>> ReadAsync();
    Task<Category> FindAsync(int id);
    Task<bool> UpdateAsync(Category entity);
    Task<bool> UpdateManyAsync(IEnumerable<Category> entities);
    Task<bool> ExistsAsync(Category entity);
    Task<bool> CheckBeforeSavingAsync(Category entity);
    // Task<IEnumerable<CategoryViewModel>> FilterAsync(Expression<Func<Category, bool>> predicate);
}

public class RepositoryCategory : RepositoryBase<Category>, IRepositoryCategory
{
    public async Task<bool> CheckBeforeSavingAsync(Category entity)
    {
        var exists = await ExistsAsync(entity);
        if (exists)
        {
            // Puedes agregar validaciones previas al update si aplica.
        }

        return await UpsertAsync(entity, exists);
    }

    public new async Task<bool> ExistsAsync(Category entity)
    {
        return await DbContext.Categories.AnyAsync(x => x.CategoryId == entity.CategoryId);
    }
}