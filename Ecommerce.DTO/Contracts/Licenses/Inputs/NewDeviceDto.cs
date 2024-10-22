using Microsoft.AspNetCore.Http;
using System;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class NewDeviceDto
    {
        public int CustomerId { get; set; }

        public int SubscriptionId { get; set; }

        public string DeviceName { get; set; }

        public string Serial { get; set; }

        public DateTime RenewalDate { get; set; }

        public IFormFile File { get; set; }
    }
}
