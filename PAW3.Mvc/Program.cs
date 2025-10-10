using System.IO;
using PAW3.Architecture;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;
using PAW3.ServiceLocator.Services;
using PAW3.ServiceLocator.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Agrega configuracion externa (appsettings de PAW3.ServiceLocator)
var serviceLocatorSettingsPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", "PAW3.ServiceLocator", "appsettings.json")
);
// optional:false => si no existe, que falle para enterarte
builder.Configuration.AddJsonFile(serviceLocatorSettingsPath, optional: false, reloadOnChange: true);


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRestProvider, RestProvider>();
builder.Services.AddScoped<IDogDataService, DogDataService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IServiceLocatorService, ServiceLocatorService>();
builder.Services.AddScoped<IServiceMapper, ServiceMapper>();
builder.Services.AddScoped<IService<ProductDTO>, ProductService>();
builder.Services.AddScoped<IService<CategoryDTO>, CategoryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();