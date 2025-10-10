using Microsoft.EntityFrameworkCore;
using PAW3.Data.Models;

namespace PAW3.Data.Repositories;

public interface IRepositoryComponent
{
    Task<bool> UpsertAsync(Component entity, bool isUpdating);
    Task<bool> CreateAsync(Component entity);
    Task<bool> DeleteAsync(Component entity);
    Task<IEnumerable<Component>> ReadAsync();
    Task<Component> FindAsync(decimal id);
    Task<bool> UpdateAsync(Component entity);
    Task<bool> UpdateManyAsync(IEnumerable<Component> entities);
    Task<bool> ExistsAsync(Component entity);
    Task<bool> CheckBeforeSavingAsync(Component entity);
}

public class RepositoryComponent : RepositoryBase<Component>, IRepositoryComponent
{
    public async Task<bool> CheckBeforeSavingAsync(Component entity)
    {
        var exists = await ExistsAsync(entity);
        return await UpsertAsync(entity, exists);
    }

    public new async Task<bool> ExistsAsync(Component entity)
    {
        return await DbContext.Components.AnyAsync(x => x.Id == entity.Id);
    }

    // Sobrecargar FindAsync para decimal Id si en base es numeric(18,0)
    public async Task<Component> FindAsync(decimal id)
    {
        return await DbContext.Components.FirstOrDefaultAsync(c => c.Id == id)!;
    }
}