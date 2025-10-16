using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class ComponentController : Controller
    {
        private readonly ILogger<ComponentController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public ComponentController(
            ILogger<ComponentController> logger,
            IServiceLocatorService serviceLocator,
            IConfiguration config,
            IRestProvider rest)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
            _config = config;
            _rest = rest;
        }

        public async Task<IActionResult> Index(decimal? id)
        {
            if (id.HasValue)
            {
                var one = await GetByIdAsync(id.Value);
                var list = one is null ? Enumerable.Empty<ComponentDTO>() : new[] { one };
                if (one is null) ViewBag.Info = $"No se encontró Component Id {id.Value}.";
                return View("~/Views/Home/Component.cshtml", list);
            }

            var all = await _serviceLocator.GetDataAsync<ComponentDTO>("component");
            return View("~/Views/Home/Component.cshtml", all);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateComponent.cshtml", new ComponentDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComponentDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateComponent.cshtml", dto);

            var baseUrl = GetApiBaseUrl("Component");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PostAsync(baseUrl, json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo crear el componente.";
                return View("~/Views/Home/CreateComponent.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Component failed");
                ViewBag.ErrorMessage = "Ocurrió un error al crear el componente.";
                return View("~/Views/Home/CreateComponent.cshtml", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(decimal id)
        {
            var comp = await GetByIdAsync(id);
            if (comp is null) return NotFound();
            return View("~/Views/Home/UpdateComponent.cshtml", comp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ComponentDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateComponent.cshtml", dto);

            var baseUrl = GetApiBaseUrl("Component");
            try
            {
                var json = JsonProvider.Serialize(dto);
                // Firma tipo: PutAsync(baseUrl, id, json)
                var resp = await _rest.PutAsync(baseUrl, dto.Id.ToString(), json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo actualizar el componente.";
                return View("~/Views/Home/UpdateComponent.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Component failed");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar el componente.";
                return View("~/Views/Home/UpdateComponent.cshtml", dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            var baseUrl = GetApiBaseUrl("Component");
            await _rest.DeleteAsync(baseUrl, id.ToString());
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private string GetApiBaseUrl(string key)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            return apisFirst?[key] ?? throw new ApplicationException($"Missing APIS[0]:{key} in ServiceLocator appsettings.json");
        }

        private async Task<ComponentDTO?> GetByIdAsync(decimal id)
        {
            var baseUrl = GetApiBaseUrl("Component");
            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<ComponentDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<ComponentDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}