using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.ServiceLocator.Helper;
using Microsoft.Extensions.Configuration;

namespace PAW3.Mvc.ServiceLocator;

public interface IServiceLocatorService
{
    Task<IEnumerable<T>> GetDataAsync<T>(string name); // utiliza "product" o "category"
}

public class ServiceLocatorService : IServiceLocatorService
{
    private readonly IRestProvider _restProvider;
    private readonly IServiceMapper _serviceMapper;
    private readonly IConfiguration _config;

    public ServiceLocatorService(IRestProvider restProvider, IServiceMapper serviceMapper, IConfiguration config)
    {
        _restProvider = restProvider;
        _serviceMapper = serviceMapper;
        _config = config;
    }

    public async Task<IEnumerable<T>> GetDataAsync<T>(string name)
    {
        // normaliza clave: Product/Category
        var key = char.ToUpper(name[0]) + name.Substring(1).ToLower();

        var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
        var baseUrl = apisFirst?[key]
            ?? throw new ApplicationException($"Missing APIS[0]:{key} in ServiceLocator appsettings.json");

        var response = await _restProvider.GetAsync(baseUrl, null);
        return await JsonProvider.DeserializeAsync<IEnumerable<T>>(response);
    }
}