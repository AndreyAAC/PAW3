using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController(ICategoryBusiness categoryBusiness) : ControllerBase
    {
        // GET: api/CategoryApi
        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            return await categoryBusiness.GetCategories(id: null);
        }

        // GET: api/CategoryApi/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Category>> Get(int id)
        {
            return await categoryBusiness.GetCategories(id);
        }

        // POST: api/CategoryApi
        [HttpPost]
        public async Task<bool> Post([FromBody] Category category)
        {
            if (category is null) return false;
            return await categoryBusiness.SaveCategoryAsync(category);
        }

        // PUT: api/CategoryApi/5
        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] Category category)
        {
            if (category is null) return false;
            category.CategoryId = id;
            return await categoryBusiness.SaveCategoryAsync(category);
        }

        // DELETE: api/CategoryApi/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await categoryBusiness.DeleteCategoryAsync(id);
        }
    }
}