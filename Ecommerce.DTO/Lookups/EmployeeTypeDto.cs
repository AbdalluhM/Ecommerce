using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Lookups
{
   
    public class EmployeeTypeDto : BaseDto
    {
    }

    #region Input 

    public class CreateEmployeeTypeInputDto : EmployeeTypeDto
    {
        public string Type { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateEmployeeTypeInputDto : CreateEmployeeTypeInputDto
    {
        public int Id { get; set; }



    }

    public class GetEmployeeTypeInputDto : EmployeeTypeDto
    {
        public int Id { get; set; }

    }


    #endregion
    #region Output

    public class GetEmployeeTypeOutputDto : EmployeeTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }

    }
    #endregion
}
