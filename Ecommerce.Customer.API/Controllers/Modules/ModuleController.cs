using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Modules;

namespace Ecommerce.Customer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ModuleController : ControllerBase
    {
        private readonly IModuleBLL _moduleBLL;

        public ModuleController(IModuleBLL moduleBLL)
        {
            _moduleBLL = moduleBLL;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModuleDetailsAsync(int id)
        {
            var response = await _moduleBLL.GetModuleDetailsAsync(id);

            return Ok(response);
        }
    }
}
