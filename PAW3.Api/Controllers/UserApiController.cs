using Microsoft.AspNetCore.Mvc;
using PAW3.Core.BusinessLogic;
using PAW3.Data.Models;

namespace PAW3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController(IUserBusiness userBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await userBusiness.GetUsers(id: null);
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<User>> Get(int id)
        {
            return await userBusiness.GetUsers(id);
        }

        [HttpPost]
        public async Task<bool> Post([FromBody] User user)
        {
            if (user is null) return false;
            return await userBusiness.SaveUserAsync(user);
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] User user)
        {
            if (user is null) return false;
            user.UserId = id;
            return await userBusiness.SaveUserAsync(user);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await userBusiness.DeleteUserAsync(id);
        }
    }
}