using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.Addons.AddonSliders.Inputs
{
    public class NewAddonSliderDto
    {
        public int AddonId { get; set; }

        public IFormFile Image { get; set; }
    }
}
