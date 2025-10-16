using PAW3.Data.Models;
using PAW3.Data.Repositories;

namespace PAW3.Core.BusinessLogic;

public interface ICategoryBusiness
{
    Task<bool> DeleteCategoryAsync(int id);
    Task<IEnumerable<Category>> GetCategories(int? id);
    Task<bool> SaveCategoryAsync(Category category);
}

public class CategoryBusiness(IRepositoryCategory repositoryCategory) : ICategoryBusiness
{
    public async Task<bool> SaveCategoryAsync(Category category)
    {
        var entity = await repositoryCategory.FindAsync(category.CategoryId);

        if (entity == null)
        {
            // Crear
            category.LastModified = category.LastModified ?? DateTime.UtcNow;
            return await repositoryCategory.CreateAsync(category);
        }
        else
        {
            // Actualizar
            entity.CategoryName = category.CategoryName;
            entity.Description = category.Description;
            entity.LastModified = category.LastModified ?? DateTime.UtcNow;
            entity.ModifiedBy = category.ModifiedBy;

            return await repositoryCategory.UpdateAsync(entity);
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var entity = await repositoryCategory.FindAsync(id);
        return entity is null ? false : await repositoryCategory.DeleteAsync(entity);
    }

    public async Task<IEnumerable<Category>> GetCategories(int? id)
    {
        return id is null
            ? await repositoryCategory.ReadAsync()
            : [await repositoryCategory.FindAsync((int)id)];
    }
}