using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public CategoryController(
            ILogger<CategoryController> logger,
            IServiceLocatorService serviceLocator,
            IConfiguration config,
            IRestProvider rest)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
            _config = config;
            _rest = rest;
        }

        // LIST (con filtro opcional por id)
        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var idSearch = await GetCategoryByIdAsync(id.Value);
                var list = idSearch is null ? Enumerable.Empty<CategoryDTO>() : new[] { idSearch };

                if (idSearch is null)
                    ViewBag.Info = $"No se encontró la Categoría {id.Value}.";

                return View("~/Views/Home/Category.cshtml", list);
            }

            var categories = await _serviceLocator.GetDataAsync<CategoryDTO>("category");
            return View("~/Views/Home/Category.cshtml", categories);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateCategory.cshtml", new CategoryDTO
            {
                LastModified = DateTime.UtcNow,
                ModifiedBy = "Andrey"
            });
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateCategory.cshtml", category);

            if (category.LastModified == default)
                category.LastModified = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(category.ModifiedBy))
                category.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("Category");

            try
            {
                var json = JsonProvider.Serialize(category);
                var response = await _rest.PostAsync(baseUrl, json);

                var success = bool.TryParse(response, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(response);
                if (success)
                {
                    TempData["Toast"] = "Categoría creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ErrorMessage = "No se pudo crear la categoría.";
                return View("~/Views/Home/CreateCategory.cshtml", category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Category failed");
                ViewBag.ErrorMessage = "Ocurrió un error al crear la categoría.";
                return View("~/Views/Home/CreateCategory.cshtml", category);
            }
        }

        // UPDATE (GET)
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category is null) return NotFound();

            return View("~/Views/Home/UpdateCategory.cshtml", category);
        }

        // UPDATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateCategory.cshtml", category);

            if (category.LastModified == default)
                category.LastModified = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(category.ModifiedBy))
                category.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("Category");

            try
            {
                var json = JsonProvider.Serialize(category);
                // Firma tipo: PutAsync(baseUrl, id, json) — igual que tu Get/Delete
                var putResponse = await _rest.PutAsync(baseUrl, category.CategoryId.ToString(), json);

                var success = bool.TryParse(putResponse, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(putResponse);
                if (success)
                {
                    TempData["Toast"] = $"Categoría {category.CategoryId} actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ErrorMessage = "No se pudo actualizar la categoría.";
                return View("~/Views/Home/UpdateCategory.cshtml", category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Category failed");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar la categoría.";
                return View("~/Views/Home/UpdateCategory.cshtml", category);
            }
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var baseUrl = GetApiBaseUrl("Category");
            await _rest.DeleteAsync(baseUrl, id.ToString());
            TempData["Toast"] = $"Categoría {id} eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // -------- Helpers ----------
        private string GetApiBaseUrl(string key)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            var url = apisFirst?[key] ?? throw new ApplicationException($"Missing APIS[0]:{key} in ServiceLocator appsettings.json");
            return url;
        }

        private async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var baseUrl = GetApiBaseUrl("Category");
            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<CategoryDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<CategoryDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}