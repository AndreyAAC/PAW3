using Microsoft.EntityFrameworkCore;
using PAW3.Data.Models;

namespace PAW3.Data.Repositories;

public interface IRepositoryInventory
{
    Task<bool> UpsertAsync(Inventory entity, bool isUpdating);
    Task<bool> CreateAsync(Inventory entity);
    Task<bool> DeleteAsync(Inventory entity);
    Task<IEnumerable<Inventory>> ReadAsync();
    Task<Inventory> FindAsync(int id);
    Task<bool> UpdateAsync(Inventory entity);
    Task<bool> UpdateManyAsync(IEnumerable<Inventory> entities);
    Task<bool> ExistsAsync(Inventory entity);
    Task<bool> CheckBeforeSavingAsync(Inventory entity);
}

public class RepositoryInventory : RepositoryBase<Inventory>, IRepositoryInventory
{
    public async Task<bool> CheckBeforeSavingAsync(Inventory entity)
    {
        var exists = await ExistsAsync(entity);
        // punto para agregar reglas previas si aplica
        return await UpsertAsync(entity, exists);
    }

    public new async Task<bool> ExistsAsync(Inventory entity)
    {
        return await DbContext.Inventories.AnyAsync(x => x.InventoryId == entity.InventoryId);
    }
}