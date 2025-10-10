using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentApiController(IComponentBusiness componentBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Component>> Get()
        {
            return await componentBusiness.GetComponents(id: null);
        }

        [HttpGet("{id:decimal}")]
        public async Task<IEnumerable<Component>> Get(decimal id)
        {
            return await componentBusiness.GetComponents(id);
        }

        [HttpPost]
        public async Task<bool> Post([FromBody] Component component)
        {
            if (component is null) return false;
            return await componentBusiness.SaveComponentAsync(component);
        }

        [HttpPut("{id:decimal}")]
        public async Task<bool> Put(decimal id, [FromBody] Component component)
        {
            if (component is null) return false;
            component.Id = id;
            return await componentBusiness.SaveComponentAsync(component);
        }

        [HttpDelete("{id:decimal}")]
        public async Task<bool> Delete(decimal id)
        {
            return await componentBusiness.DeleteComponentAsync(id);
        }
    }
}