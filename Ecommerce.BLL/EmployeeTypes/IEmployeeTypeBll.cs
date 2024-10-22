using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.EmployeeTypes
{
    public interface IEmployeeTypeBLL
    {
        Task<IResponse<List<GetEmployeeTypeOutputDto>>> GetAllEmployeeTypesAsync();
        IResponse<List<GetEmployeeTypeOutputDto>> GetAllEmployeeTypes();
        IResponse<GetEmployeeTypeOutputDto> Create(CreateEmployeeTypeInputDto inputDto);
        IResponse<GetEmployeeTypeOutputDto> Update(UpdateEmployeeTypeInputDto inputDto );
        IResponse<bool> Delete(DeleteEntityInputDto inputDto);
    }
}
