

using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.WorkSpaces
{
    public class CreatWorkSpaceApiDto
    {
        public string Name { get; set; }
        public IFormFile? File { get; set; }
        public int DexefCountryId { get; set; }
        public int DexefCurrencyId { get; set; }
        public int DatabaseTypeId { get; set; }
        public bool IsCloud { get; set; }
        public bool IsDefault { get; set; }
        public int? VersionSubscriptionId { get; set; }
        public string? Descreption { get; set; }
        public string? ServerIP { get; set; }
        public string? DatabaseName { get; set; }
    }
}
