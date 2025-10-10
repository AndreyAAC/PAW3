using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public RoleController(
            ILogger<RoleController> logger,
            IServiceLocatorService serviceLocator,
            IConfiguration config,
            IRestProvider rest)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
            _config = config;
            _rest = rest;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var one = await GetByIdAsync(id.Value);
                var list = one is null ? Enumerable.Empty<RoleDTO>() : new[] { one };
                if (one is null) ViewBag.Info = $"No se encontró el RoleId {id.Value}.";
                return View("~/Views/Home/Role.cshtml", list);
            }

            var all = await _serviceLocator.GetDataAsync<RoleDTO>("role");
            return View("~/Views/Home/Role.cshtml", all);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateRole.cshtml", new RoleDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateRole.cshtml", dto);

            var baseUrl = GetApiBaseUrl("Role");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PostAsync(baseUrl, json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo crear el rol.";
                return View("~/Views/Home/CreateRole.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Role failed");
                ViewBag.ErrorMessage = "Ocurrió un error al crear el rol.";
                return View("~/Views/Home/CreateRole.cshtml", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var role = await GetByIdAsync(id);
            if (role is null) return NotFound();
            return View("~/Views/Home/UpdateRole.cshtml", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateRole.cshtml", dto);

            var baseUrl = GetApiBaseUrl("Role");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PutAsync(baseUrl, dto.RoleId.ToString(), json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo actualizar el rol.";
                return View("~/Views/Home/UpdateRole.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Role failed");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar el rol.";
                return View("~/Views/Home/UpdateRole.cshtml", dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var baseUrl = GetApiBaseUrl("Role");
            await _rest.DeleteAsync(baseUrl, id.ToString());
            return RedirectToAction(nameof(Index));
        }

        private string GetApiBaseUrl(string key)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            return apisFirst?[key] ?? throw new ApplicationException($"Missing APIS[0]:{key}");
        }

        private async Task<RoleDTO?> GetByIdAsync(int id)
        {
            var baseUrl = GetApiBaseUrl("Role");
            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<RoleDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<RoleDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}