using System;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseLogDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string ActionType { get; set; }

        public int? OldStatusId { get; set; }

        public string OldStatus { get; set; }

        public int? NewStatusId { get; set; }

        public string NewStatus { get; set; }

        public string Owner { get; set; }
    }
}
