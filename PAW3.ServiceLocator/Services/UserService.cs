using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.ServiceLocator.Services.Contracts;

namespace PAW3.ServiceLocator.Services;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetDataAsync();
}

public class UserService(IRestProvider restProvider, IConfiguration configuration)
    : IService<UserDTO>, IUserService
{
    public async Task<IEnumerable<UserDTO>> GetDataAsync()
    {
        var url = configuration.GetStringFromAppSettings("APIS", "User");
        var response = await restProvider.GetAsync(url, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<UserDTO>>(response);
    }
}