using PAW3.Data.Models;
using PAW3.Data.Repositories;

namespace PAW3.Core.BusinessLogic;

public interface IInventoryBusiness
{
    Task<bool> DeleteInventoryAsync(int id);
    Task<IEnumerable<Inventory>> GetInventories(int? id);
    Task<bool> SaveInventoryAsync(Inventory inv);
}

public class InventoryBusiness(IRepositoryInventory repositoryInventory) : IInventoryBusiness
{
    public async Task<bool> SaveInventoryAsync(Inventory inv)
    {
        var entity = await repositoryInventory.FindAsync(inv.InventoryId);
        if (entity == null)
        {
            inv.DateAdded ??= DateTime.UtcNow;
            inv.LastUpdated ??= DateTime.UtcNow;
            return await repositoryInventory.CreateAsync(inv);
        }
        else
        {
            entity.UnitPrice = inv.UnitPrice;
            entity.UnitsInStock = inv.UnitsInStock;
            entity.LastUpdated = inv.LastUpdated ?? DateTime.UtcNow;
            entity.DateAdded = inv.DateAdded ?? entity.DateAdded;
            entity.ModifiedBy = inv.ModifiedBy;

            return await repositoryInventory.UpdateAsync(entity);
        }
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        var entity = await repositoryInventory.FindAsync(id);
        return entity is null ? false : await repositoryInventory.DeleteAsync(entity);
    }

    public async Task<IEnumerable<Inventory>> GetInventories(int? id)
    {
        return id is null
            ? await repositoryInventory.ReadAsync()
            : [await repositoryInventory.FindAsync((int)id)];
    }
}