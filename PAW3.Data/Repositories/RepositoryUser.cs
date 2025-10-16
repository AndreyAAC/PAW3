using Microsoft.EntityFrameworkCore;
using PAW3.Data.Models;

namespace PAW3.Data.Repositories;

public interface IRepositoryUser
{
    Task<bool> UpsertAsync(User entity, bool isUpdating);
    Task<bool> CreateAsync(User entity);
    Task<bool> DeleteAsync(User entity);
    Task<IEnumerable<User>> ReadAsync();
    Task<User> FindAsync(int id);
    Task<bool> UpdateAsync(User entity);
    Task<bool> UpdateManyAsync(IEnumerable<User> entities);
    Task<bool> ExistsAsync(User entity);
    Task<bool> CheckBeforeSavingAsync(User entity);
}

public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
{
    public async Task<bool> CheckBeforeSavingAsync(User entity)
    {
        var exists = await ExistsAsync(entity);
        return await UpsertAsync(entity, exists);
    }

    public new async Task<bool> ExistsAsync(User entity)
    {
        return await DbContext.Users.AnyAsync(x => x.UserId == entity.UserId);
    }
}