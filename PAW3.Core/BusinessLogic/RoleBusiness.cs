using PAW3.Data.Models;
using PAW3.Data.Repositories;

namespace PAW3.Core.BusinessLogic;

public interface IRoleBusiness
{
    Task<bool> DeleteRoleAsync(int id);
    Task<IEnumerable<Role>> GetRoles(int? id);
    Task<bool> SaveRoleAsync(Role role);
}

public class RoleBusiness(IRepositoryRole repositoryRole) : IRoleBusiness
{
    public async Task<bool> SaveRoleAsync(Role role)
    {
        var entity = await repositoryRole.FindAsync(role.RoleId);

        if (entity == null)
        {
            return await repositoryRole.CreateAsync(role);
        }
        else
        {
            entity.RoleName = role.RoleName;
            return await repositoryRole.UpdateAsync(entity);
        }
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var entity = await repositoryRole.FindAsync(id);
        return entity is null ? false : await repositoryRole.DeleteAsync(entity);
    }

    public async Task<IEnumerable<Role>> GetRoles(int? id)
    {
        return id is null
            ? await repositoryRole.ReadAsync()
            : [await repositoryRole.FindAsync((int)id)];
    }
}