using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Customers.Reviews.Customers;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Customers.DownloadCenter
{
    public class DownloadCenterBLL : BaseBLL, IDownloadCenterBLL
    {

        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<ApplicationTag> _applicationTagRepository;
        IRepository<AddOn> _addOnRepository;
        IRepository<AddOnTag> _addOnTagRepository;
        IRepository<VersionAddon> _versionAddonRepository;
        IRepository<Core.Entities.Version> _versionRepository;

        IRepository<Feature> _featureRepository;
        IRepository<VersionFeature> _versionFeatureRepository;
        IRepository<ViewApplicationVersionFeature> _applicationVersionFeatureRepository;
        IRepository<CountryCurrency> _countryCurrencyRepository;
        private readonly ICustomerReviewBLL _customerReviewBll;

        #endregion
        #region Constructor
        public DownloadCenterBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Feature> featureRepository, IRepository<VersionFeature> versionFeatureRepository, IRepository<Application> applicationRepository, IRepository<Core.Entities.Version> versionRepository, IRepository<ViewApplicationVersionFeature> applicationVersionFeatureRepository, ICustomerReviewBLL customerReviewBll, IRepository<AddOn> addOnRepository, IRepository<VersionAddon> versionAddonRepository, IRepository<CountryCurrency> countryCurrencyRepository, IRepository<AddOnTag> addOnTagRepository, IRepository<ApplicationTag> applicationTagRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _featureRepository = featureRepository;
            _versionRepository = versionRepository;
            _versionFeatureRepository = versionFeatureRepository;
            _applicationVersionFeatureRepository = applicationVersionFeatureRepository;
            _customerReviewBll = customerReviewBll;
            _addOnRepository = addOnRepository;
            _versionAddonRepository = versionAddonRepository;
            _countryCurrencyRepository = countryCurrencyRepository;
            _addOnTagRepository = addOnTagRepository;
            _applicationTagRepository = applicationTagRepository;
        }


        #endregion


        #region Basic CRUD 

        public async Task<IResponse<GetDownloadCenterApplicationsOutputDto>> GetByIdAsync(int ApplicationId)
        {
            var output = new Response<GetDownloadCenterApplicationsOutputDto>();

            try
            {
                ////Input Validation
                //var validator = new GetApplicationVersionOutputDto().Validate(inputDto);
                //if (!validator.IsValid)
                //{
                //    return output.CreateResponse(validator.Errors);
                //}

                //Business Validation             
                var applicationEntity = _applicationRepository.Where(x => x.Id == ApplicationId).FirstOrDefault();
                // 1- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var result = _mapper.Map<GetDownloadCenterApplicationsOutputDto>(applicationEntity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetDownloadCenterApplicationsOutputDto>>> GetAllAsync()
        {
            var output = new Response<List<GetDownloadCenterApplicationsOutputDto>>();

            var result = _versionRepository
                .Where(x => x.Application != null)
                .AsEnumerable()
                .GroupBy(x => x.ApplicationId)
                    .Select(a => new GetDownloadCenterApplicationsOutputDto
                    {
                        Id = a.Key,// a.FirstOrDefault()?.Version?.ApplicationId ?? ,
                        Name = a.FirstOrDefault().Application?.Name,
                        Title = a.FirstOrDefault().Application?.Title,
                        Logo = _mapper.Map<FileStorageDto>(a.FirstOrDefault()?.Application.Image),
                        LongDescription = a.FirstOrDefault().Application?.LongDescription,
                        ShortDescription = a.FirstOrDefault().Application?.ShortDescription,
                        MainPageUrl = a.FirstOrDefault().Application?.MainPageUrl,
                        SubscriptionTypeId = a.FirstOrDefault().Application?.SubscriptionTypeId ?? 0,
                        SubscriptionType = a.FirstOrDefault().Application?.SubscriptionType?.Name,
                        Rate = _customerReviewBll.GetRateAsync(a.Key, false).GetAwaiter().GetResult(),
                        Versions = _mapper.Map<List<GetDownloadCenterApplicationVersionOutputDto>>(a.ToList())
                    })
                   .ToList();

            return output.CreateResponse(result);

        }
        public async Task<IResponse<PagedResultDto<GetDownloadCenterApplicationsOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetDownloadCenterApplicationsOutputDto>>();

            //keep original skip & max Result Counts
            int originalSkip = pagedDto.SkipCount;
            int originalMaxResultCount = pagedDto.MaxResultCount;
            //update skip & max Result Counts to search in cross table before group by
            pagedDto.SkipCount = 0;
            pagedDto.MaxResultCount = int.MaxValue;

            var data = GetPagedList<GetDownloadCenterApplicationsOutputDto, Application, int>(pagedDto: pagedDto,
                repository: _applicationRepository, orderExpression: x => x.Id,
              searchExpression:
               x =>
                 string.IsNullOrWhiteSpace(pagedDto.SearchTerm) ||
                 (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) && (x.Title.Contains(pagedDto.SearchTerm) || (x.ApplicationTags.Any(x => x.Tag.Name.Contains(pagedDto.SearchTerm))))),
              sortDirection: pagedDto.SortingDirection,
              disableFilter: false);
            return output.CreateResponse(data);

        }



        public async Task<IResponse<DownloadCenterResultDto>> GetDownloadCenterAsync(string searchTerm, int countryId, int? applicationTagId = null, int? addOnTagId = null)
        {
            var output = new Response<DownloadCenterResultDto>();

            try
            {
                var currencyData = _countryCurrencyRepository.Where(c => c.CountryId == countryId || c.DefaultForOther).Select(c => new
                {
                    c.CurrencyId,
                    c.Currency.Code
                }).FirstOrDefault();
                searchTerm = searchTerm?.Trim().ToLower().Replace(oldValue: "%20", " ");

                // Get Applications
                var applicationList = _applicationRepository
                    //Get Only Applications Has 1 or more versions
                    .Where(x => x.Versions.Any())
                    .WhereIf(a => a.ApplicationTags.Select(at => at.TagId).Contains(applicationTagId ?? 0), applicationTagId != null)
                    .AsEnumerable();

                var applicationDto = _mapper.Map<List<DownloadCenterApplicationResultDto>>(
                    applicationList,
                    opt =>
                    {
                        opt.Items["CurrencyId"] = currencyData.CurrencyId;
                        opt.Items["CurrencyCode"] = currencyData.Code;
                    });


                var activeAddOns = _addOnRepository
                    .GetAll()
                    .WhereIf(a => a.AddOnTags.Select(at => at.TagId).Contains(addOnTagId ?? 0), addOnTagId != null)
                    .Select(x => x.Id)
                    .ToList();

                var relatedAddons =
                   _versionAddonRepository
                   //Filter by Application
                   .Where(x => activeAddOns.Contains(x.AddonId) && applicationList.Select(a => a.Id).Contains(x.Version.ApplicationId))
                   .Select(x => x.Addon)
                   //.WhereIf(a => a.Title.Contains(searchTerm), !string.IsNullOrEmpty(searchTerm))
                   .Distinct()
                   .AsEnumerable();

                // var relatedAddonDto = _mapper.Map<IEnumerable<DownloadCenterAddonResultDto>>(relatedAddons);
                var relatedAddonDto = _mapper.Map<IEnumerable<DownloadCenterAddonResultDto>>(
                relatedAddons, opt =>
                {
                    opt.Items["CurrencyId"] = currencyData.CurrencyId;
                    opt.Items["CurrencyCode"] = currencyData.Code;
                });


                //foreach (var relatedAddon in relatedAddonDto)
                //{
                //    relatedAddon.CurrencyCode = currencyData.Code;
                //}
                output.Data = new DownloadCenterResultDto
                {
                    Applications = applicationDto,
                    AddOns = relatedAddonDto
                };

                if (!string.IsNullOrEmpty(searchTerm))
                {

                    output.Data.Applications = output.Data.Applications.Where(x => (x.Title != null && x.Title.ToLower().Contains(searchTerm)));


                    output.Data.AddOns = output.Data.AddOns.Where(x => (x.Title != null && x.Title.ToLower().Contains(searchTerm)));
                }


                return output;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetApplicationTagOutputDto>>> GetApplicationTagsAsync()
        {

            var response = new Response<List<GetApplicationTagOutputDto>>();
            try
            {
                var result = _applicationTagRepository.GetAll().AsEnumerable();
                return response.CreateResponse(_mapper.Map<List<GetApplicationTagOutputDto>>(result));
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IResponse<List<GetAddonTagOutputDto>>> GetAddOnTagsAsync()
        {
            var response = new Response<List<GetAddonTagOutputDto>>();
            try
            {
                var result = _addOnTagRepository.GetAll().AsEnumerable();
                return response.CreateResponse(_mapper.Map<List<GetAddonTagOutputDto>>(result));
            }
            catch (Exception e)
            {

                throw;
            }
        }






        #endregion

    }
}
