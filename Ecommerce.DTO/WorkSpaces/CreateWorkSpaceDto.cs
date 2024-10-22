

using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Ecommerce.DTO.Files;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.WorkSpaces
{
    public class CreateWorkSpaceDto
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string Alias => Name;
        [JsonIgnore]
        public string ConnectionString { get; set; }
        [JsonIgnore]
        public int CustomerId { get; set; }
        public int DexefCountryId { get; set; }
        public int DexefCurrencyId { get; set; }
        public int DatabaseTypeId { get; set; }
        public int? VersionSubscriptionId { get; set; }
        public bool IsCloud { get; set; }
        public IFormFile File { get; set; }
        [JsonIgnore]
        public FileDto? FileDto { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; }
        [CanBeNull]
        public string Descreption { get; set; }
        [CanBeNull] public string ServerIP { get; set; }

        [CanBeNull] public string DatabaseName { get; set; }

    }
}
