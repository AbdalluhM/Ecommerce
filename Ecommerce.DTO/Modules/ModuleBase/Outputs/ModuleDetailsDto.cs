using System.Collections.Generic;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;

namespace Ecommerce.DTO.Modules.ModuleBase.Outputs
{
    public class ModuleDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public FileStorageDto Logo { get; set; } = new();

        public string Description { get; set; }

        public List<string> SlidersPath { get; set; } = new();

        public List<AppVersionDto> AvailableAppVersions { get; set; } = new();

        public IEnumerable<GetTagOutputDto> Tags { get; set; } = new List<GetTagOutputDto>();
    }
}