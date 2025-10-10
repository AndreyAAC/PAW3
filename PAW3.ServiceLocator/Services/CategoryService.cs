using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.ServiceLocator.Services.Contracts;

namespace PAW3.ServiceLocator.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetDataAsync();
}

public class CategoryService(IRestProvider restProvider, IConfiguration configuration) : IService<CategoryDTO>, ICategoryService
{
    public async Task<IEnumerable<CategoryDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "Category");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<CategoryDTO>>(response);
    }
}