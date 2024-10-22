using System;

namespace Ecommerce.DTO.WorkSpaces
{
    public class WorkspaceDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string ConnectionString { get; set; }

        public int CountryId { get; set; }

        public int CurrencyId { get; set; }

        public bool IsCloud { get; set; }
        public string? Descreption { get; set; }
        public string? ServerIP { get; set; }
        public string? DatabaseName { get; set; }
        public string? DatabaseFileName => DatabaseName + "_Files";

        public DateTime CreateDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
