using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryApiController(IInventoryBusiness inventoryBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Inventory>> Get()
        {
            return await inventoryBusiness.GetInventories(id: null);
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Inventory>> Get(int id)
        {
            return await inventoryBusiness.GetInventories(id);
        }

        [HttpPost]
        public async Task<bool> Post([FromBody] Inventory inv)
        {
            if (inv is null) return false;
            return await inventoryBusiness.SaveInventoryAsync(inv);
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] Inventory inv)
        {
            if (inv is null) return false;
            inv.InventoryId = id;
            return await inventoryBusiness.SaveInventoryAsync(inv);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await inventoryBusiness.DeleteInventoryAsync(id);
        }
    }
}