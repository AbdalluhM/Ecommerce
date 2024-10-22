using Ecommerce.Core.Entities;
using Ecommerce.DTO.Applications.ApplicationBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationModules;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Ecommerce.DTO.Applications.ApplicationVersions
{
    public class VersionDetailsDto
    {
        public int Id { get; set; }

        public int DownloadCount { get; set; }
        public VersionPriceDetailsDto Price { get; set; } = new();
        [CanBeNull] public string DownloadUrl { get; set; }

        public IEnumerable<AppModuleInfoDto> Modules { get; set; } = new List<AppModuleInfoDto>();

        public IEnumerable<AppAddonInfoDto> Addons { get; set; } = new List<AppAddonInfoDto>();
    }
}
