using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Modules;
using Ecommerce.BLL.Tags;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.Customer.API.Controllers.Auth;

using System.Threading.Tasks;

using Twilio.Jwt.Taskrouter;

namespace Ecommerce.Customer.API.Controllers.LookUps
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LookUpController : ControllerBase
    {
        #region Fields

        private readonly IModuleBLL _moduleBLL;
        private readonly ITagBLL _tagBLL;
        private readonly IVersionBLL _versionBLL;

        #endregion
        #region Constructor

        public LookUpController(IModuleBLL moduleBLL,
                                ITagBLL tagBLL,
                                IVersionBLL versionBLL)
        {
            _moduleBLL = moduleBLL;
            _tagBLL = tagBLL;
            _versionBLL = versionBLL;
        }
        #endregion

        [HttpGet]
        [Route("Modules/GetAll")]
        public async Task<IActionResult> GetAllModulesAsync( )
        {
            var result = await _moduleBLL.GetAllAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("Tags/GetAll")]
        public async Task<IActionResult> GetAllTagsAsync( )
        {
            var result =  _tagBLL.GetAllList();
            return Ok(result);
        }

        [HttpGet("Versions/GetAll")]
        public async Task<IActionResult> GetAppVersionsAsync(int appId)
        {
            var response =  await _versionBLL.GetAppVersionsLookupAsync(appId);

            return Ok(response);
        }
    }
}

