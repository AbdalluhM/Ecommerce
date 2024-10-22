using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.DevicesType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.DevicesTypee
{
    public interface IDevicesTypeBLL
    {
        Task<IResponse<List<GetDevicesTypeDto>>> GetDevicesType(int customerId);
    }
}
