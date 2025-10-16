using Microsoft.AspNetCore.Mvc;
using PAW3.Architecture;
using PAW3.Architecture.Providers;
using PAW3.Models.DTOs;
using PAW3.Mvc.ServiceLocator;
using PAW3.ServiceLocator.Helper;

namespace PAW3.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IServiceLocatorService _serviceLocator;
        private readonly IConfiguration _config;
        private readonly IRestProvider _rest;

        public UserController(
            ILogger<UserController> logger,
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
                var list = one is null ? Enumerable.Empty<UserDTO>() : new[] { one };
                if (one is null) ViewBag.Info = $"No se encontró UserId {id.Value}.";
                return View("~/Views/Home/User.cshtml", list);
            }

            var all = await _serviceLocator.GetDataAsync<UserDTO>("user");
            return View("~/Views/Home/User.cshtml", all);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Home/CreateUser.cshtml", new UserDTO
            {
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsActive = true,
                ModifiedBy = "Andrey"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/CreateUser.cshtml", dto);

            dto.CreatedAt ??= DateTime.UtcNow;
            dto.LastModified ??= DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(dto.ModifiedBy)) dto.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("User");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PostAsync(baseUrl, json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo crear el usuario.";
                return View("~/Views/Home/CreateUser.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create User failed");
                ViewBag.ErrorMessage = "Ocurrió un error al crear el usuario.";
                return View("~/Views/Home/CreateUser.cshtml", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await GetByIdAsync(id);
            if (user is null) return NotFound();
            return View("~/Views/Home/UpdateUser.cshtml", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/UpdateUser.cshtml", dto);

            dto.LastModified ??= DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(dto.ModifiedBy)) dto.ModifiedBy = "Andrey";

            var baseUrl = GetApiBaseUrl("User");
            try
            {
                var json = JsonProvider.Serialize(dto);
                var resp = await _rest.PutAsync(baseUrl, dto.UserId.ToString(), json);
                var success = bool.TryParse(resp, out var parsed) ? parsed : !string.IsNullOrWhiteSpace(resp);
                if (success) return RedirectToAction(nameof(Index));

                ViewBag.ErrorMessage = "No se pudo actualizar el usuario.";
                return View("~/Views/Home/UpdateUser.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update User failed");
                ViewBag.ErrorMessage = "Ocurrió un error al actualizar el usuario.";
                return View("~/Views/Home/UpdateUser.cshtml", dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var baseUrl = GetApiBaseUrl("User");
            await _rest.DeleteAsync(baseUrl, id.ToString());
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private string GetApiBaseUrl(string key)
        {
            var apisFirst = _config.GetSection("APIS").GetChildren().FirstOrDefault();
            return apisFirst?[key] ?? throw new ApplicationException($"Missing APIS[0]:{key} in ServiceLocator appsettings.json");
        }

        private async Task<UserDTO?> GetByIdAsync(int id)
        {
            var baseUrl = GetApiBaseUrl("User");
            var response = await _rest.GetAsync(baseUrl, id.ToString());
            try
            {
                return await JsonProvider.DeserializeAsync<UserDTO>(response);
            }
            catch
            {
                var list = await JsonProvider.DeserializeAsync<IEnumerable<UserDTO>>(response);
                return list?.FirstOrDefault();
            }
        }
    }
}