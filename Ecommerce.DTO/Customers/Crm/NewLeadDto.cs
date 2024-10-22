using System;

namespace Ecommerce.DTO.Customers.Crm
{
    public class CrmNewLeadDto
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CountryId { get; set; }

        public string CurrencyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SoftwareId { get; set; }

        public int LeadSourceCode { get; set; }

        public int IndustryCode { get; set; }

        public int FollowType { get; set; }

        public int TaxRegistrationNumber { get; set; }

        public int CompanySize { get; set; }

        public string Website { get; set; }
    }
}
