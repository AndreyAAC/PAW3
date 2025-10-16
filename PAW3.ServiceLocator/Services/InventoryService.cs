using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.ServiceLocator.Services.Contracts;

namespace PAW3.ServiceLocator.Services;

public interface IInventoryService
{
    Task<IEnumerable<InventoryDTO>> GetDataAsync();
}

public class InventoryService(IRestProvider restProvider, IConfiguration configuration)
    : IService<InventoryDTO>, IInventoryService
{
    public async Task<IEnumerable<InventoryDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Inventory");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<InventoryDTO>>(response);
    }
}