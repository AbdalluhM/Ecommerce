namespace Ecommerce.DTO.Employees
{
    public class EmployeeCountryDto : BaseDto
    {
    }
    #region Input 

    public class AssignEmployeeToCountryInputDto : BaseDto
    {
        public int EmployeeId { get; set; }
        public int CountryCurrencyId { get; set; }
    }

    public class UpdateAssignEmployeeToCountryInputDto : AssignEmployeeToCountryInputDto
    {
        public int Id { get; set; }

    


    }

    public class GetAssignedEmployeeToCountryInputDto : BaseDto
    {
        public int Id { get; set; }

    }


    #endregion
    #region Output

    public class GetAssignedEmployeeToCountryOutputDto
    {
        public int Id { get; set; }
        public int CountryCurrencyId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeUserName { get; set; }
        public string CountryName { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
    }
    #endregion
}
