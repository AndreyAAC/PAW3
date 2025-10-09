using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.Models;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public ProductController(
            ILogger<ProductController> logger,
            IServiceLocatorService serviceLocator,
            IConfiguration config,
            IRestProvider rest)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
            _config = config;
            _rest = rest;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var products = await _serviceLocator.GetDataAsync<ProductDTO>("product");
            var vm = new HomeViewModel { Title = "Product Page", Products = products };
            return View("~/Views/Home/Product.cshtml", vm);
        }

        // CREATE (GET) - Formulario vacío con defaults
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateProduct.cshtml", new ProductDTO
            {
                LastModified = DateTime.UtcNow,
                ModifiedBy = "Andrey"
            });
        }

        // CREATE (POST) - Envía el nuevo producto al API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateProduct.cshtml", product);

            if (product.LastModified == default)
                product.LastModified = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(product.ModifiedBy))
                product.ModifiedBy = "Andrey";

            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            var baseUrl = apisFirst?["Product"]
                ?? throw new ApplicationException("Missing APIS[0]:Product in ServiceLocator appsettings.json");

            try
            {
                var json = JsonProvider.Serialize(product);
                // PostAsync devuelve string => parseamos para decidir éxito
                var response = await _rest.PostAsync(baseUrl, json);

                bool success = false;
                if (bool.TryParse(response, out var parsed))
                    success = parsed;
                else
                    success = !string.IsNullOrWhiteSpace(response); // éxito si hay contenido

                if (success)
                {
                    TempData["Toast"] = "Producto creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ErrorMessage = "No se pudo crear el producto. Intenta nuevamente.";
                return View("~/Views/Home/CreateProduct.cshtml", product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto vía API");
                ViewBag.ErrorMessage = "Ocurrió un error al crear el producto.";
                return View("~/Views/Home/CreateProduct.cshtml", product);
            }
        }

        // UPDATE (GET) - Carga el producto en el formulario
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product is null) return NotFound();

            return View("~/Views/Home/UpdateProduct.cshtml", product);
        }

        // UPDATE (POST) - Envía los cambios al API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateProduct.cshtml", product);

            if (product.LastModified == default)
                product.LastModified = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(product.ModifiedBy))
                product.ModifiedBy = "Andrey";

            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            var baseUrl = apisFirst?["Product"]
                ?? throw new ApplicationException("Missing APIS[0]:Product in ServiceLocator appsettings.json");

            try
            {
                var json = JsonProvider.Serialize(product);

                // PutAsync devuelve string (asumido) => parseamos
                // Firma asumida: PutAsync(baseUrl, id, json) como Get/Delete
                var putResponse = await _rest.PutAsync(baseUrl, product.ProductId.ToString(), json);

                bool success = false;
                if (bool.TryParse(putResponse, out var parsed))
                    success = parsed;
                else
                    success = !string.IsNullOrWhiteSpace(putResponse);

                if (success)
                {
                    TempData["Toast"] = $"Producto {product.ProductId} actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ErrorMessage = "No se pudo actualizar el producto. Intenta nuevamente.";
                return View("~/Views/Home/UpdateProduct.cshtml", product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto vía API");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar el producto.";
                return View("~/Views/Home/UpdateProduct.cshtml", product);
            }
        }

        // DELETE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            var baseUrl = apisFirst?["Product"]
                ?? throw new ApplicationException("Missing APIS[0]:Product in ServiceLocator appsettings.json");

            await _rest.DeleteAsync(baseUrl, id.ToString());
            TempData["Toast"] = $"Producto {id} eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // Helper - Obtener producto por ID
        private async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            var baseUrl = apisFirst?["Product"]
                ?? throw new ApplicationException("Missing APIS[0]:Product in ServiceLocator appsettings.json");

            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<ProductDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<ProductDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}