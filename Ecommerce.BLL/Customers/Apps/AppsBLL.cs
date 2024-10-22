using AutoMapper;

using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Customers.Apps
{
    public class AppsBLL : BaseBLL, IAppsBLL
    {

        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<Core.Entities.Version> _versionRepository;
        IRepository<VersionModule> _versionModuleRepository;

        IRepository<VersionAddon> _versionAddonRepository;
        IRepository<AddOn> _addOnRepository;

        IRepository<VersionPrice> _vesionPriceRepository;

        IRepository<AddOnPrice> _addOnPriceRepository;
        IRepository<Tag> _tagRepository;
        IRepository<ApplicationTag> _applicationTagRepository;
        IRepository<AddOnTag> _addOnTagRepository;

        ICountryBLL _countryBLL;

        private readonly IMemoryCache _cache;

        private const string cachKey = "FilterApps";

        #endregion
        #region Constructor
        public AppsBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<AddOn> addOnRepository, IRepository<VersionAddon> versionAddOnRepository, IRepository<Application> applicationRepository, IRepository<Core.Entities.Version> versionRepository, IRepository<VersionModule> versionModuleRepository, IRepository<ViewApplicationVersionFeature> applicationVersionFeatureRepository, IRepository<AddOnPrice> addOnPriceRepository, IRepository<VersionPrice> vesionPriceRepository, IRepository<Tag> tagRepository, IRepository<ApplicationTag> applicationTagRepository, IRepository<AddOnTag> addOnTagRepository, ICountryBLL countryBLL, IMemoryCache cache) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _addOnRepository = addOnRepository;
            _versionRepository = versionRepository;
            _versionAddonRepository = versionAddOnRepository;
            _versionModuleRepository = versionModuleRepository;
            _addOnPriceRepository = addOnPriceRepository;
            _vesionPriceRepository = vesionPriceRepository;
            _tagRepository = tagRepository;
            _applicationTagRepository = applicationTagRepository;
            _addOnTagRepository = addOnTagRepository;
            _countryBLL = countryBLL;
            _cache = cache;
        }


        #endregion


        #region Basic CRUD 

        public async Task<IResponse<BrowseAppsOutputDto>> GetByIdAsync(int ApplicationId)
        {
            var output = new Response<BrowseAppsOutputDto>();

            try
            {

                //Business Validation             
                var applicationEntity = _applicationRepository.Where(x => x.Id == ApplicationId).FirstOrDefault();
                // 1- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));


                BrowseAppsApplicationOutputDto applicationDto = _mapper.Map<BrowseAppsApplicationOutputDto>(applicationEntity);
                var relatedAddons = _versionAddonRepository.Where(x => x.Version.ApplicationId == applicationDto.Id)
                    .Select(x => x.Addon)
                    .Distinct()
                    .AsEnumerable();
                var relatedAddonDto = _mapper.Map<IEnumerable<BrowseAppsAddOnOutputDto>>(relatedAddons);

                output.Data = new BrowseAppsOutputDto
                {
                    Applications = new List<BrowseAppsApplicationOutputDto> { applicationDto },
                    AddOns = relatedAddonDto
                };
                return output;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetPriceRangeOutputDto>> GetPriceRangeAsync(int? ApplicationId = null)
        {
            var output = new Response<GetPriceRangeOutputDto>();

            try
            {
                //Business Validation
                var exists = _applicationRepository
                    .WhereIf(x => x.Id == ApplicationId, ApplicationId.HasValue && ApplicationId.Value > 0)
                    .Any();
                // 1- Check if Entity Exists
                if (!exists)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));
                //Business


                var versionPrices = _vesionPriceRepository
                   .WhereIf(x => x.Version.ApplicationId == ApplicationId, ApplicationId.HasValue && ApplicationId.Value > 0)
                   .AsEnumerable()
                   .GroupBy(g => g.Version?.ApplicationId)
                   .Select(x => new GetApplicationPriceRangeOutputDto
                   {
                       ApplicationId = ApplicationId.GetValueOrDefault(),
                       MinForeverPrice = x.Min(x => x.ForeverNetPrice),
                       MaxForeverPrice = x.Max(x => x.ForeverNetPrice),
                       MinYearlyPrice = x.Min(x => x.YearlyNetPrice),
                       MaxYearlyPrice = x.Max(x => x.YearlyNetPrice),
                       MinMonthlyPrice = x.Min(x => x.MonthlyNetPrice),
                       MaxMonthlyPrice = x.Max(x => x.MonthlyNetPrice)
                   })
                    .AsEnumerable();


                var addOnPrices = _addOnPriceRepository
                    .Where(x => x.AddOn != null && x.AddOn.VersionAddons.Any(va => va.Version != null))
                    .WhereIf(x => x.AddOn != null && x.AddOn.VersionAddons.Any(va => va.Version != null && va.Version.ApplicationId == ApplicationId.Value), ApplicationId.HasValue)
                    .AsEnumerable()
                    .GroupBy(g => g.AddOnId)
                    .Select(x => new GetApplicationPriceRangeOutputDto
                    {
                        ApplicationId = ApplicationId.GetValueOrDefault(),
                        MinForeverPrice = x.Min(x => x.ForeverNetPrice),
                        MaxForeverPrice = x.Max(x => x.ForeverNetPrice),
                        MinYearlyPrice = x.Min(x => x.YearlyNetPrice),
                        MaxYearlyPrice = x.Max(x => x.YearlyNetPrice),
                        MinMonthlyPrice = x.Min(x => x.MonthlyNetPrice),
                        MaxMonthlyPrice = x.Max(x => x.MonthlyNetPrice)
                    })
                     .AsEnumerable();

                var outputList = versionPrices.ToList();
                outputList.AddRange(addOnPrices);



                output.Data = new GetPriceRangeOutputDto()
                {
                    Ranges = outputList
                };

                return output;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<BrowseAppsOutputDto>> GetAllAsync(string searchTerm = null)
        {
            var output = new Response<BrowseAppsOutputDto>();

            try
            {

                //Business
                // Get customer countryId
                // Get Applications
                var applicationList = _applicationRepository.WhereIf(x => x.Title.Contains(searchTerm) ||
                x.ApplicationTags.Any(x => x.Tag.Name.Contains(searchTerm)), !string.IsNullOrWhiteSpace(searchTerm)).ToList();


                // Get Related AddOns 
                List<BrowseAppsApplicationOutputDto> applicationDto = _mapper.Map<List<BrowseAppsApplicationOutputDto>>(applicationList);
                var relatedAddons =
                    _versionAddonRepository
                    .Where(x => x.Version != null && x.Version.Application != null && x.Version.VersionReleases.Any() &&
                    applicationList.Select(x => x.Id).Contains(x.Version.ApplicationId))
                    .WhereIf(x => x.Addon.Title.Contains(searchTerm) || x.Addon.AddOnTags.Any(x => x.Tag.Name.Contains(searchTerm)), !string.IsNullOrWhiteSpace(searchTerm))
                    .Select(x => x.Addon)
                    .Distinct()
                    .AsEnumerable();
                var relatedAddonDto = _mapper.Map<IEnumerable<BrowseAppsAddOnOutputDto>>(relatedAddons);

                output.Data = new BrowseAppsOutputDto
                {
                    Applications = applicationDto,
                    AddOns = relatedAddonDto
                };
                return output;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #region Old FilterApps
        //public async Task<IResponse<BrowseAppsOutputDto>> FilterApps( FilterAppsInputDto filter )
        //{
        //    var output = new Response<BrowseAppsOutputDto>();

        //    try
        //    {


        //        //Business
        //        //get applicationIds Related to ModuleIds
        //        var appRelatedToModules = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
        //            .WhereIf(x => x.Module != null && filter.ModuleIds.Contains(x.ModuleId), filter.ModuleIds is not null)
        //            .Select(x => x.Version.ApplicationId)
        //            .Distinct()
        //            .ToList();

        //        filter.SearchTerm = filter.SearchTerm?.Trim();
        //        // Get Applications
        //        var applicationList = _applicationRepository
        //            //Get Only Applications Has 1 or more versions
        //            .Where(x => x.Versions.Any())
        //            //Filter by SubscriptionType
        //            .WhereIf(x => x.SubscriptionTypeId == filter.SubscriptionTypeId.Value, filter.SubscriptionTypeId.HasValue && filter.SubscriptionTypeId.Value > 0)
        //            //Filter by Tags
        //            .WhereIf(x => x.ApplicationTags.Any(x => x.Tag != null && filter.TagIds.Contains(x.TagId)), filter.TagIds is not null)
        //            //Filter by ModuleIds(applicationIds Related to ModuleIds)
        //            .WhereIf(x => appRelatedToModules.Contains(x.Id), filter.ModuleIds is not null)
        //            //Filter by SearchTerm
        //            //.WhereIf(x => x.Title.Contains(filter.SearchTerm)
        //            //|| x.ApplicationTags.Any(t => t.Tag.Name.Contains(filter.SearchTerm)
        //            //|| x.Versions.Any(v => v.Title.Contains(filter.SearchTerm))
        //            //|| x.Versions.Any(va => va.VersionAddons.Any(a => a.Addon.Title.Contains(filter.SearchTerm)))), !string.IsNullOrWhiteSpace(filter.SearchTerm))
        //            .AsEnumerable();


        //        // Get Related AddOns 
        //        IEnumerable<BrowseAppsApplicationOutputDto> applicationDto = _mapper.Map<List<BrowseAppsApplicationOutputDto>>(applicationList);
        //        var relatedAddons =
        //            _versionAddonRepository
        //            //Filter by Application
        //            .Where(x => x.Addon != null && applicationList.Select(a => a.Id).Contains(x.Version.ApplicationId))
        //            //Filter by Tags
        //            .WhereIf(x => x.Addon.AddOnTags.Any(x => x.AddOn != null && filter.TagIds.Contains(x.TagId)), filter.TagIds is not null)
        //            .Select(x => x.Addon)
        //            //Filter by SearchTerm
        //            //.WhereIf(x => x.Title.Contains(filter.SearchTerm) || x.AddOnTags.Any(x => x.Tag.Name.Contains(filter.SearchTerm)
        //            //|| x.AddOn.Name.Contains(filter.SearchTerm)), !string.IsNullOrWhiteSpace(filter.SearchTerm))
        //            .Distinct()
        //            .AsEnumerable();


        //        var relatedAddonDto = _mapper.Map<IEnumerable<BrowseAppsAddOnOutputDto>>(relatedAddons);

        //        //Apply Price
        //        applicationDto = applicationDto
        //            .WhereIf(x => x.Price != null && x.Price?.NetPrice >= filter.PriceFrom.Value, filter.PriceFrom.HasValue)
        //            .WhereIf(x => x.Price != null && x.Price?.NetPrice <= filter.PriceTo.Value, filter.PriceTo.HasValue)
        //            .AsEnumerable();
        //        relatedAddonDto = relatedAddonDto
        //            //Get Only AddOns that have viewed Prices in search
        //            .Where(x => applicationDto.Select(a => a.Id).Intersect(x.ApplicationId).Count() > 0)
        //            .WhereIf(x => x.Price != null && x.Price.NetPrice >= filter.PriceFrom.Value, filter.PriceFrom.HasValue)
        //            .WhereIf(x => x.Price != null && x.Price.NetPrice <= filter.PriceTo.Value, filter.PriceTo.HasValue)
        //            .AsEnumerable()
        //            ;


        //        #region Flag WishListed Applications & AddOns
        //        //Flag wishListed Applications
        //        var wishListApplicationIds = _applicationRepository
        //            .Where(x => applicationDto.Select(a => a.Id)
        //            .Contains(x.Id) && x.WishListApplications.Any(x => x.CustomerId == filter.CurrentCustomerId))
        //            .Select(x => x.Id)
        //            .ToList();
        //        applicationDto.ToList().ForEach(async x =>
        //        {
        //            if (wishListApplicationIds.Contains(x.Id))
        //                x.IsWishListed = true;
        //        });

        //        //Flag wishListed AddOns
        //        var wishListAddOnIds = _addOnRepository
        //            .Where(x => relatedAddonDto.Select(a => a.Id)
        //            .Contains(x.Id) && x.WishListAddOns.Any(x => x.CustomerId == filter.CurrentCustomerId))
        //            .Select(x => x.Id)
        //            .ToList();
        //        relatedAddonDto.ToList().ForEach(x =>
        //        {
        //            if (wishListAddOnIds.Contains(x.Id))
        //                x.IsWishListed = true;
        //        });
        //        #endregion



        //        output.Data = new BrowseAppsOutputDto
        //        {
        //            Applications = applicationDto,
        //            AddOns = relatedAddonDto
        //        };

        //        //filter searchterm 
        //        if (!string.IsNullOrEmpty(filter.SearchTerm))
        //        {
        //            output.Data.Applications = output.Data.Applications.Where(x => x.Title.Contains(filter.SearchTerm) ||
        //                                                                       (x.FeaturedTag != null && x.FeaturedTag.Contains(filter.SearchTerm)) ||
        //                                                                         x.Name.Contains(filter.SearchTerm));

        //            output.Data.AddOns = output.Data.AddOns.Where(x => x.Title.Contains(filter.SearchTerm) ||
        //                                                            (x.FeaturedTag != null && x.FeaturedTag.Contains(filter.SearchTerm)) ||
        //                                                             x.Name.Contains(filter.SearchTerm));
        //        }

        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        return output.CreateResponse(ex);
        //    }
        //}
        #endregion
        public async Task<IResponse<BrowseAppsOutputDto>> FilterApps(FilterAppsInputDto filter)
        {
            var output = new Response<BrowseAppsOutputDto>();
            //Response<BrowseAppsOutputDto> cachedData;
            try
            {

                //if (!_cache.TryGetValue(cachKey, out cachedData))
                //{
                //Business
                //get applicationIds Related to ModuleIds
                var appRelatedToModules = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Module != null && filter.ModuleIds.Contains(x.ModuleId), filter.ModuleIds is not null)
                .Select(x => x.Version.ApplicationId)
                .Distinct()
                .ToList();

                filter.SearchTerm = filter.SearchTerm?.Trim().ToLower().Replace(oldValue: "%20", " ");

                List<int> applicationIdByTags = new List<int>();
                List<int> addOnIdByTags = new List<int>();
                #region Tags
                var activeTags = _tagRepository
                      .GetAll()
                      .WhereIf(x => filter.TagIds.Contains(x.Id), filter.TagIds != null)
                      .Select(x => x.Id)
                      .ToList()
                      ;
                var searchTags = _tagRepository
                      .GetAll()
                      .WhereIf(x => x.Name.ToLower().Contains(filter.SearchTerm), !string.IsNullOrWhiteSpace(filter.SearchTerm))
                       .Select(x => x.Id)
                     .ToList();
                activeTags = activeTags.Union(searchTags).Distinct().ToList();
                #endregion

                applicationIdByTags = _applicationTagRepository
                                          .Where(x => activeTags.Contains(x.TagId))
                                          .WhereIf(x => filter.TagIds.Contains(x.TagId), filter.TagIds != null)
                                          .Select(x => x.ApplicationId).ToList();

                addOnIdByTags = _addOnTagRepository
                    .Where(x => activeTags.Contains(x.TagId))
                    .WhereIf(x => filter.TagIds.Contains(x.TagId), filter.TagIds != null)
                    .Select(x => x.AddOnId)
                    .ToList();

                // Get Applications
                var applicationList = _applicationRepository
                    //Get Only Applications Has 1 or more versions
                    .Where(x => x.Versions.Any())
                    //Filter by SubscriptionType
                    .WhereIf(x => x.SubscriptionTypeId == filter.SubscriptionTypeId.Value, filter.SubscriptionTypeId.HasValue && filter.SubscriptionTypeId.Value > 0)
                     //Filter by Tags
                     .WhereIf(x => applicationIdByTags.Contains(x.Id), filter.TagIds is not null)
                    //Filter by ModuleIds(applicationIds Related to ModuleIds)
                    .WhereIf(x => appRelatedToModules.Contains(x.Id), filter.ModuleIds is not null)
                    //Filter by SearchTerm
                    //.WhereIf(x => x.Title.Contains(filter.SearchTerm), !string.IsNullOrWhiteSpace(filter.SearchTerm))
                    .AsEnumerable();


                // Get Related AddOns 
                IEnumerable<BrowseAppsApplicationOutputDto> applicationDto = _mapper.Map<List<BrowseAppsApplicationOutputDto>>(applicationList);


                var activeAddOns = _addOnRepository
                    .GetAll()
                    .Select(x => x.Id)
                    .ToList();
                var relatedAddons =
                    _versionAddonRepository
                    //Filter by Application
                    .Where(x => activeAddOns.Contains(x.AddonId) && applicationList.Select(a => a.Id).Contains(x.Version.ApplicationId))
                    //Filter by Tags
                    .WhereIf(x => addOnIdByTags.Contains(x.Id), filter.TagIds is not null)
                    .Select(x => x.Addon)
                    //Filter by SearchTerm
                    //.WhereIf(x => x.Title.Contains(filter.SearchTerm), !string.IsNullOrWhiteSpace(filter.SearchTerm))
                    .Distinct()
                    .AsEnumerable();


                var relatedAddonDto = _mapper.Map<IEnumerable<BrowseAppsAddOnOutputDto>>(relatedAddons);

                //Apply Price
                applicationDto = applicationDto
                    .WhereIf(x => x.Price != null && x.Price?.NetPrice >= filter.PriceFrom.Value, filter.PriceFrom.HasValue)
                    .WhereIf(x => x.Price != null && x.Price?.NetPrice <= filter.PriceTo.Value, filter.PriceTo.HasValue)
                    .AsEnumerable();
                relatedAddonDto = relatedAddonDto
                    //Get Only AddOns that have viewed Prices in search
                    .Where(x => applicationDto.Select(a => a.Id).Intersect(x.ApplicationId).Count() > 0)
                    .WhereIf(x => x.Price != null && x.Price.NetPrice >= filter.PriceFrom.Value, filter.PriceFrom.HasValue)
                    .WhereIf(x => x.Price != null && x.Price.NetPrice <= filter.PriceTo.Value, filter.PriceTo.HasValue)
                    .AsEnumerable()
                    ;


                #region Flag WishListed Applications & AddOns
                //Flag wishListed Applications
                var wishListApplicationIds = _applicationRepository
                    .Where(x => applicationDto.Select(a => a.Id)
                    .Contains(x.Id) && x.WishListApplications.Any(x => x.CustomerId == filter.CurrentCustomerId))
                    .Select(x => x.Id)
                    .ToList();
                applicationDto.ToList().ForEach(async x =>
                {
                    if (wishListApplicationIds.Contains(x.Id))
                        x.IsWishListed = true;
                });

                //Flag wishListed AddOns
                var wishListAddOnIds = _addOnRepository
                    .Where(x => relatedAddonDto.Select(a => a.Id)
                    .Contains(x.Id) && x.WishListAddOns.Any(x => x.CustomerId == filter.CurrentCustomerId))
                    .Select(x => x.Id)
                    .ToList();
                relatedAddonDto.ToList().ForEach(x =>
                {
                    if (wishListAddOnIds.Contains(x.Id))
                        x.IsWishListed = true;
                });
                #endregion



                output.Data = new BrowseAppsOutputDto
                {
                    Applications = applicationDto,
                    AddOns = relatedAddonDto
                };

                //filter searchterm 
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {

                    output.Data.Applications = output.Data.Applications.Where(x => (x.Title != null && x.Title.ToLower().Contains(filter.SearchTerm)) ||
                   (x.ApplicationTags.Where(x => x.Name != null).Any(at => at.Name.ToLower().Contains(filter.SearchTerm))));
                    //(x.FeaturedTag != null &&  x.FeaturedTag.ToLower().Contains(filter.SearchTerm))||
                    //  x.Name.ToLower().Contains(filter.SearchTerm)
                    // );

                    output.Data.AddOns = output.Data.AddOns.Where(x => (x.Title != null && x.Title.ToLower().Contains(filter.SearchTerm)) ||
                                                          (x.AddOnTags.Where(x => x.Name != null).Any(at => at.Name.ToLower().Contains(filter.SearchTerm))));
                    //(x.FeaturedTag != null && x.FeaturedTag.ToLower().Contains(filter.SearchTerm)) ||
                    // x.Name.ToLower().Contains(filter.SearchTerm)
                    // );
                }

                // cachedData = output;
                // Set cache options
                //var cacheEntryOptions = new MemoryCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Keep for 5 minutes
                //    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // Maximum cache lifetime


                // Save the data in cache
                // _cache.Set(cachKey, cachedData, cacheEntryOptions);

                // return cachedData;
                //}
                return output;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        #endregion

    }
}
