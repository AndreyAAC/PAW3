using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController(IProductBusiness productBusiness) : ControllerBase
    {
        // GET: api/<ProductApiController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await productBusiness.GetProducts(id: null);
        }

        // GET api/<ProductApiController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Product>> Get(int id)
        {
            return await productBusiness.GetProducts(id);
        }

        // POST api/<ProductApiController>
        [HttpPost]
        public async Task<bool> Post([FromBody] Product product)
        {
            if (product is null)
                return false;
            return await productBusiness.SaveProductAsync(product);
        }

        // PUT api/<ProductApiController>/5
        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] Product product)
        {
            if (product is null)
                return false;

            product.ProductId = id;

            return await productBusiness.SaveProductAsync(product);
        }

        // DELETE api/<ProductApiController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await productBusiness.DeleteProductAsync(id);
        }
    }
}
