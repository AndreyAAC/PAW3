using PAW3.Data.Models;
using PAW3.Data.Repositories;

namespace PAW3.Core.BusinessLogic;

public interface IUserBusiness
{
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<User>> GetUsers(int? id);
    Task<bool> SaveUserAsync(User user);
}

public class UserBusiness(IRepositoryUser repositoryUser) : IUserBusiness
{
    public async Task<bool> SaveUserAsync(User user)
    {
        var entity = await repositoryUser.FindAsync(user.UserId);

        if (entity == null)
        {
            user.CreatedAt ??= DateTime.UtcNow;
            user.LastModified ??= DateTime.UtcNow;
            return await repositoryUser.CreateAsync(user);
        }
        else
        {
            entity.Username = user.Username;
            entity.Email = user.Email;
            entity.PasswordHash = user.PasswordHash;
            entity.IsActive = user.IsActive;
            entity.LastModified = user.LastModified ?? DateTime.UtcNow;
            entity.ModifiedBy = user.ModifiedBy;
            entity.RoleId = user.RoleId;

            return await repositoryUser.UpdateAsync(entity);
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var entity = await repositoryUser.FindAsync(id);
        return entity is null ? false : await repositoryUser.DeleteAsync(entity);
    }

    public async Task<IEnumerable<User>> GetUsers(int? id)
    {
        return id is null
            ? await repositoryUser.ReadAsync()
            : [await repositoryUser.FindAsync((int)id)];
    }
}