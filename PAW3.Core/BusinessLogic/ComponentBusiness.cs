using PAW3.Data.Models;
using PAW3.Data.Repositories;

namespace PAW3.Core.BusinessLogic;

public interface IComponentBusiness
{
    Task<bool> DeleteComponentAsync(decimal id);
    Task<IEnumerable<Component>> GetComponents(decimal? id);
    Task<bool> SaveComponentAsync(Component comp);
}

public class ComponentBusiness(IRepositoryComponent repositoryComponent) : IComponentBusiness
{
    public async Task<bool> SaveComponentAsync(Component comp)
    {
        var entity = await repositoryComponent.FindAsync(comp.Id);
        if (entity == null)
        {
            return await repositoryComponent.CreateAsync(comp);
        }
        else
        {
            entity.Name = comp.Name;
            entity.Content = comp.Content;
            return await repositoryComponent.UpdateAsync(entity);
        }
    }

    public async Task<bool> DeleteComponentAsync(decimal id)
    {
        var entity = await repositoryComponent.FindAsync(id);
        return entity is null ? false : await repositoryComponent.DeleteAsync(entity);
    }

    public async Task<IEnumerable<Component>> GetComponents(decimal? id)
    {
        return id is null
            ? await repositoryComponent.ReadAsync()
            : [await repositoryComponent.FindAsync(id.Value)];
    }
}