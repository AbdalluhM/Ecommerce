using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.DTO.Customers.DevicesType;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.DevicesTypee
{
    public class DevicesTypeBLL : IDevicesTypeBLL
    {
        IMapper _mapper;
        IRepository<DevicesType> _devicesTypeReposatory;
        IRepository<VersionSubscription> _versionSubscriptionReposatory;
        IRepository<Core.Entities.Version> _versionRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<VersionPrice> _versionPriceRepository;
        public DevicesTypeBLL(IMapper mapper, IRepository<DevicesType> devicesTypeReposatory, IRepository<VersionSubscription> versionSubscriptionReposatory, IRepository<Core.Entities.Version> versionReposatory, IRepository<VersionPrice> versionPriceRepository, IRepository<Application> applicationRepository)
        {
            _mapper = mapper;

            _devicesTypeReposatory = devicesTypeReposatory;
            _versionSubscriptionReposatory = versionSubscriptionReposatory;
            _versionRepository = versionReposatory;
            _versionPriceRepository = versionPriceRepository;
            _applicationRepository = applicationRepository;
        }
        public async Task<IResponse<List<GetDevicesTypeDto>>> GetDevicesType(int customerId)
        {
            var output = new Response<List<GetDevicesTypeDto>>();
            try
            {


                var devicesType = await _devicesTypeReposatory.GetAllListAsync();
                var mappedDevice = _mapper.Map<List<GetDevicesTypeDto>>(devicesType);

                foreach (var device in mappedDevice)
                {

                    var versionSubscriptions = _versionSubscriptionReposatory.Where(vs => vs.CustomerSubscription.CustomerId == customerId &&
                                           vs.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid) &&
                                          _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                         .FirstOrDefault(x => x.Id == _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                         .FirstOrDefault(e => e.Id == _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                         .FirstOrDefault(e => e.Id == vs.VersionPriceId).VersionId).ApplicationId).DeviceTypeId == device.Id).ToList();

                    device.DevicesCount = versionSubscriptions.Sum(x => x.CustomerSubscription.Licenses.Count);

                    device.NumberOfDevices = versionSubscriptions.Sum(vs => vs.CustomerSubscription.NumberOfLicenses);

                }
                return output.CreateResponse(mappedDevice);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
    }
}
