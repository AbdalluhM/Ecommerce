using Ecommerce.DTO.Paging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Addons
{
    public class AddOnFilteredPagedResult : FilteredResultRequestDto
    {
        public int AddOnId { get; set; }
    }

}
