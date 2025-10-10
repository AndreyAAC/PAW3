using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public InventoryController(
            ILogger<InventoryController> logger,
            IServiceLocatorService serviceLocator,
            IConfiguration config,
            IRestProvider rest)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
            _config = config;
            _rest = rest;
        }

        // LIST + search by id
        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var idSearch = await GetByIdAsync(id.Value);
                var list = idSearch is null ? Enumerable.Empty<InventoryDTO>() : new[] { idSearch };
                if (idSearch is null) ViewBag.Info = $"No se encontró InventoryId {id.Value}.";
                return View("~/Views/Home/Inventory.cshtml", list);
            }

            var all = await _serviceLocator.GetDataAsync<InventoryDTO>("inventory");
            return View("~/Views/Home/Inventory.cshtml", all);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateInventory.cshtml", new InventoryDTO
            {
                DateAdded = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                ModifiedBy = "Andrey"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateInventory.cshtml", dto);

            dto.DateAdded ??= DateTime.UtcNow;
            dto.LastUpdated ??= DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(dto.ModifiedBy)) dto.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("Inventory");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var response = await _rest.PostAsync(baseUrl, json);
                var success = bool.TryParse(response, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(response);

                if (success) return RedirectToAction(nameof(Index));
                ViewBag.ErrorMessage = "No se pudo crear el inventario.";
                return View("~/Views/Home/CreateInventory.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Inventory failed");
                ViewBag.ErrorMessage = "Ocurrió un error al crear el inventario.";
                return View("~/Views/Home/CreateInventory.cshtml", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var inv = await GetByIdAsync(id);
            if (inv is null) return NotFound();
            return View("~/Views/Home/UpdateInventory.cshtml", inv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(InventoryDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateInventory.cshtml", dto);

            dto.LastUpdated ??= DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(dto.ModifiedBy)) dto.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("Inventory");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PutAsync(baseUrl, dto.InventoryId.ToString(), json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);

                if (success) return RedirectToAction(nameof(Index));
                ViewBag.ErrorMessage = "No se pudo actualizar el inventario.";
                return View("~/Views/Home/UpdateInventory.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Inventory failed");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar el inventario.";
                return View("~/Views/Home/UpdateInventory.cshtml", dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var baseUrl = GetApiBaseUrl("Inventory");
            await _rest.DeleteAsync(baseUrl, id.ToString());
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private string GetApiBaseUrl(string key)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            return apisFirst?[key] ?? throw new ApplicationException($"Missing APIS[0]:{key} in ServiceLocator appsettings.json");
        }

        private async Task<InventoryDTO?> GetByIdAsync(int id)
        {
            var baseUrl = GetApiBaseUrl("Inventory");
            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<InventoryDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<InventoryDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}