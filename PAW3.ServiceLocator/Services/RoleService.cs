using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.ServiceLocator.Services.Contracts;

namespace PAW3.ServiceLocator.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetDataAsync();
}

public class RoleService(IRestProvider restProvider, IConfiguration configuration)
    : IService<RoleDTO>, IRoleService
{
    public async Task<IEnumerable<RoleDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Role");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<RoleDTO>>(response);
    }
}