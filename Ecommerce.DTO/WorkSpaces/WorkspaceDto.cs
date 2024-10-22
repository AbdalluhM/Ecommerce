using Ecommerce.DTO.Settings.Files;
using System;
using JetBrains.Annotations;

namespace Ecommerce.DTO.WorkSpaces
{
    public class WorkspaceDto
    {
        public int Id { get; set; }

        public string Alias { get; set; }
        public string ConnectionString { get; set; }
        [CanBeNull] 
        public FileStorageDto Image { get; set; }

        public bool IsCloud { get; set; }
        public int DexefCountryId { get; set; }
        public int DexefCurrencyId { get; set; }
        public string? Descreption { get; set; }
        public string? ServerIP { get; set; }
        public string? DatabaseName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
