using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Lookups;
using Ecommerce.Repositroy.Base;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.BLL.LookUps
{
    public class LookUpBLL : BaseBLL, ILookUpBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Country> _countryRepository;
        IRepository<Currency> _currencyRepository;
        IRepository<EmployeeType> _employeeTypeRepository;
        IRepository<SubscriptionType> _subscriptionType;
        IRepository<ViewDashboardTotal> _viewDashboardTotal;
        private readonly IRepository<Industry> _industryRepository;
        private readonly IRepository<CompanySize> _companySizeRepository;
        private readonly IRepository<ContactUsHelpOption> _contactUsHelpOptionRepository;
        private readonly IRepository<DexefBranch> _dexefBranchRepository ;
        #endregion

        #region Constructor
        public LookUpBLL(IMapper mapper,
                         IUnitOfWork unitOfWork,
                         IRepository<Country> countryRepository,
                         IRepository<Currency> currencyRepository,
                         IRepository<EmployeeType> employeeTypeRepository,
                         IRepository<SubscriptionType> subscriptionType,
                         IRepository<ViewDashboardTotal> viewDashboardTotal,
                         IRepository<Industry> industryRepository,
                         IRepository<CompanySize> companySizeRepository,
                         IRepository<ContactUsHelpOption> contactUsHelpOptionRepository,
                         IRepository<DexefBranch> dexefBranchRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _employeeTypeRepository = employeeTypeRepository;
            _subscriptionType = subscriptionType;
            _viewDashboardTotal = viewDashboardTotal;
            _industryRepository = industryRepository;
            _companySizeRepository = companySizeRepository;
            _contactUsHelpOptionRepository = contactUsHelpOptionRepository;
            _dexefBranchRepository = dexefBranchRepository;
        }
        #endregion

        #region Country
        public IResponse<List<GetCountryOutputDto>> GetAllCountries()
        {
            return AsyncContext.Run(() => GetAllCountriesAsync());
        }

        public async Task<IResponse<List<GetCountryOutputDto>>> GetAllCountriesAsync()
        {
            var output = new Response<List<GetCountryOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetCountryOutputDto>>(_countryRepository./*Where(x=>x.PhoneCode != null).Distinct().*/GetAll());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region Currency
        public IResponse<List<GetCurrencyOutputDto>> GetAllCurrencies()
        {
            return AsyncContext.Run(() => GetAllCurrenciesAsync());
        }

        public async Task<IResponse<List<GetCurrencyOutputDto>>> GetAllCurrenciesAsync()
        {
            var output = new Response<List<GetCurrencyOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetCurrencyOutputDto>>(_currencyRepository.GetAllList().Distinct());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        #endregion

        #region Contact Us.
        public IResponse<IEnumerable<ContactUsLookupDto>> GetAllContactUsHelpOptions()
        {
            var response = new Response<IEnumerable<ContactUsLookupDto>>();

            var helpOptions = _mapper.Map<IEnumerable<ContactUsLookupDto>>(_contactUsHelpOptionRepository.GetAllList().Distinct());

            response.CreateResponse(helpOptions);

            return response;
        }

        public IResponse<IEnumerable<ContactUsLookupDto>> GetAllIndustries()
        {
            var response = new Response<IEnumerable<ContactUsLookupDto>>();

            var industries = _mapper.Map<IEnumerable<ContactUsLookupDto>>(_industryRepository.GetAllList().Distinct());

            response.CreateResponse(industries);

            return response;
        }

        public IResponse<IEnumerable<ContactUsLookupDto>> GetAllCompanySize()
        {
            var response = new Response<IEnumerable<ContactUsLookupDto>>();

            var companySize = _mapper.Map<IEnumerable<ContactUsLookupDto>>(_companySizeRepository.GetAllList().Distinct());

            response.CreateResponse(companySize);

            return response;
        }

        public IResponse<IEnumerable<DexefBranchDto>> GetAllDexefBranches()
        {
            var response = new Response<IEnumerable<DexefBranchDto>>();

            var branches = _mapper.Map<IEnumerable<DexefBranchDto>>(_dexefBranchRepository.GetAllList().Distinct());

            response.CreateResponse(branches);

            return response;
        }
        #endregion

        #region EmployeeType
        public IResponse<List<GetEmployeeTypeOutputDto>> GetAllEmployeeTypes()
        {
            return AsyncContext.Run(() => GetAllEmployeeTypesAsync());
        }

        public async Task<IResponse<List<GetEmployeeTypeOutputDto>>> GetAllEmployeeTypesAsync()
        {
            var output = new Response<List<GetEmployeeTypeOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetEmployeeTypeOutputDto>>(_employeeTypeRepository.GetAllList().Distinct());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        #endregion

        #region DashBoard
        public async Task<IResponse<GetDashBoardOutputDto>> GetAllDashboardCounts()
        {
            var output = new Response<GetDashBoardOutputDto>();

            try
            {
                var result = _mapper.Map<GetDashBoardOutputDto>(_viewDashboardTotal.GetAll().FirstOrDefault());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region SubscriptionType
        public IResponse<List<GetSubscriptionTypeOutputDto>> GetAllSubscriptionTypes()
        {
            return AsyncContext.Run(() => GetAllSubscriptionTypesAsync());
        }

        public async Task<IResponse<List<GetSubscriptionTypeOutputDto>>> GetAllSubscriptionTypesAsync()
        {
            var output = new Response<List<GetSubscriptionTypeOutputDto>>();

            try
            {
                var result = _mapper.Map<List<GetSubscriptionTypeOutputDto>>(_subscriptionType.GetAllList().Distinct());
                return await Task.FromResult(output.CreateResponse(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion
    }
}
