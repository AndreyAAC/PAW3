using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleApiController(IRoleBusiness roleBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Role>> Get() => await roleBusiness.GetRoles(null);

        [HttpGet("{id}")]
        public async Task<IEnumerable<Role>> Get(int id) => await roleBusiness.GetRoles(id);

        [HttpPost]
        public async Task<bool> Post([FromBody] Role role)
        {
            if (role is null) return false;
            return await roleBusiness.SaveRoleAsync(role);
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] Role role)
        {
            if (role is null) return false;
            role.RoleId = id;
            return await roleBusiness.SaveRoleAsync(role);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) => await roleBusiness.DeleteRoleAsync(id);
    }
}