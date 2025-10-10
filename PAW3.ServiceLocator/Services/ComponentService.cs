using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.ServiceLocator.Services.Contracts;

namespace PAW3.ServiceLocator.Services;

public interface IComponentService
{
    Task<IEnumerable<ComponentDTO>> GetDataAsync();
}

public class ComponentService(IRestProvider restProvider, IConfiguration configuration)
    : IService<ComponentDTO>, IComponentService
{
    public async Task<IEnumerable<ComponentDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Component");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<ComponentDTO>>(response);
    }
}