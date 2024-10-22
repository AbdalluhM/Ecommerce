using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.DTO.Contracts.Licenses
{
    public class LicensesDto
    {
        public class GetAllLicensesOutputDto
        {
            public int Id { get; set; }
            public string Customer { get; set; }
            public string ContractId { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
            public string OldDevice { get; set; }
            public string NewDevice => RequestChangeDevices
                .Where(x => x.LicencesId == Id)
                .Select(x => x.NewDevice).LastOrDefault();
            public string OldSerial { get; set; }
            public string NewSerial => RequestChangeDevices
                .Where(x => x.LicencesId == Id)
                .Select(x => x.NewSerial).LastOrDefault();
            public string Reason => RequestChangeDevices
                .Where(x => x.LicencesId == Id)
                .Select(x => x.Reason).LastOrDefault();
            public DateTime RequestDate { get; set; }
            [JsonIgnore]
            public IEnumerable<RequestChangeDevicesOutputDto> RequestChangeDevices { get; set; }
        }
      
        public class RequestChangeDevicesOutputDto
        {
            [JsonIgnore]
            public int ReasonChangeDeviceId { get; set; }
            public int LicencesId { get; set; }
            public string OldDevice { get; set; }
            public string OldSerial { get; set; }
            public string NewDevice { get; set; }
            public string NewSerial { get; set; }
            public string Reason { get; set; }  
        }
        public class ReasonChangeDeviceOutputDto
        {
            public int Id  { get; set; }
            public string Reason { get; set; }
            
        }
    }
}
