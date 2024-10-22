namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class ChangeLicenseFilterDto : LicenseRequestFilterDto
    {
        public bool IsApproved { get; set; }
    }
}
