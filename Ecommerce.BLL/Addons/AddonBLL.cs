using AutoMapper;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.Addons.AddonBase;
using Ecommerce.BLL.Validation.Addons.AddonLabels;
using Ecommerce.BLL.Validation.Addons.AddOnPrices;
using Ecommerce.BLL.Validation.Addons.AddonSilders;
using Ecommerce.BLL.Validation.Addons.AddonTags;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Addons;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonSliders.Inputs;
using Ecommerce.DTO.Addons.AddonSliders.Outputs;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;
using Ecommerce.Repositroy.Base;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Addons
{
    public class AddOnBLL : BaseBLL, IAddOnBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<AddOnTag> _addontagRepository;
        IRepository<AddOnLabel> _addonLabelRepository;
        IRepository<Tag> _tagRepository;
        private readonly IRepository<AddOn> _addonRepository;
        private readonly IRepository<AddOnSlider> _addonSliderRepository;

        private readonly IFileBLL _fileBLL;
        private readonly IBlobFileBLL _BlobfileBLL;
        IRepository<AddOnPrice> _addOnPriceRepository;
        IRepository<Core.Entities.Version> _versionRepository;
        IRepository<Application> _applicationRepository;
        IRepository<CountryCurrency> _countryCurrencyRepository;
        IRepository<PriceLevel> _priceLevelRepository;
        IRepository<ViewMissingAddOnPrice> _missingPriceRepository;
        IRepository<VersionPrice> _versionPriceRepository;
        private readonly IRepository<VersionSubscription> _versionSubscriptionRepository;

        private readonly IRepository<VersionAddon> _versionAddonRepository;

        private readonly ICountryBLL _countryBLL;
        private readonly IEmployeeBLL _employeeBLL;


        #endregion

        #region Constructor

        public AddOnBLL(IMapper mapper,
                        IUnitOfWork unitOfWork,
                        IRepository<AddOnTag> adontagRepository,
                        IRepository<AddOnLabel> addonLabelRepository,
                        IRepository<Tag> tagRepository,
                        IRepository<AddOn> addonRepository,
                        IFileBLL fileBLL,
                        IRepository<AddOnPrice> addOnPriceRepository,
                        IRepository<CountryCurrency> countryCurrencyRepository,
                        IRepository<PriceLevel> priceLevelRepository,
                        IRepository<ViewMissingAddOnPrice> missingPriceRepository,
                        IRepository<AddOnSlider> addonSliderRepository,
                        IRepository<VersionPrice> versionPriceRepository,
                        IRepository<VersionSubscription> versionSubscriptionRepository,
                        IRepository<VersionAddon> versionAddonRepository,
                        ICountryBLL countryBLL,
                        IEmployeeBLL employeeBLL,
                        IRepository<Core.Entities.Version> versionRepository,
                        IRepository<Application> applicationRepository,
                       IBlobFileBLL blobfileBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _addontagRepository = adontagRepository;
            _addonLabelRepository = addonLabelRepository;
            _tagRepository = tagRepository;
            _addonRepository = addonRepository;
            _fileBLL = fileBLL;
            _addOnPriceRepository = addOnPriceRepository;
            _countryCurrencyRepository = countryCurrencyRepository;
            _priceLevelRepository = priceLevelRepository;
            _missingPriceRepository = missingPriceRepository;
            _addonSliderRepository = addonSliderRepository;
            _versionPriceRepository = versionPriceRepository;
            _versionSubscriptionRepository = versionSubscriptionRepository;
            _versionAddonRepository = versionAddonRepository;
            _countryBLL = countryBLL;
            _employeeBLL = employeeBLL;
            _versionRepository = versionRepository;
            _applicationRepository = applicationRepository;
            _BlobfileBLL = blobfileBLL;
        }
        #endregion

        #region AddOn
        //public IResponse<bool> CreateAddon( FileDto fileDto, NewAddonDto newAddonDto, int currentUserId )
        //{
        //    var output = new Response<bool>();

        //    try
        //    {
        //        var fileValidator = new FileDtoValidator().Validate(fileDto);

        //        if (!fileValidator.IsValid)
        //        {
        //            return output.CreateResponse(fileValidator.Errors);
        //        }

        //        // => inputs validation.
        //        var validator = new NewAddonValidator().Validate(newAddonDto);

        //        if (!validator.IsValid)
        //        {
        //            return output.CreateResponse(validator.Errors);
        //        }

        //        // => business validation.
        //        //check on (name and title)
        //        if (IsAddonNameExist(GetJsonLanguageModelOrNull(newAddonDto.Name),LangEnum.Default))
        //            output.AppendError(MessageCodes.AlreadyExistsEn, nameof(newAddonDto.Name));
        //        if(IsAddonNameExist(GetJsonLanguageModelOrNull(newAddonDto.Name), LangEnum.Ar))
        //            output.AppendError(MessageCodes.AlreadyExistsAr, nameof(newAddonDto.Name));
        //        if (IsAddonTitleExist(GetJsonLanguageModelOrNull(newAddonDto.Title), LangEnum.Default))
        //            output.AppendError(MessageCodes.AlreadyExistsEn, nameof(newAddonDto.Title));
        //         if (IsAddonTitleExist(GetJsonLanguageModelOrNull(newAddonDto.Title), LangEnum.Ar))
        //            output.AppendError(MessageCodes.AlreadyExistsAr, nameof(newAddonDto.Title));

        //        if (!output.IsSuccess)
        //            return output.CreateResponse(false);

        //        // upload image.
        //        var createdFileResult = _fileBLL.UploadFile(fileDto, currentUserId);

        //        if (createdFileResult.Errors.Any())
        //        {
        //            return output.AppendErrors(createdFileResult.Errors).CreateResponse();
        //        }

        //        var addonModel = _mapper.Map<AddOn>(newAddonDto);

        //        addonModel.CreatedBy = currentUserId;
        //        addonModel.Logo = createdFileResult.Data;

        //        _addonRepository.Add(addonModel);

        //        _unitOfWork.Commit();

        //        return output.CreateResponse(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return output.CreateResponse(ex);
        //    }
        //}

        public async Task<IResponse<GetAddOnOutputDto>> CreateAsync(CreateAddOnInputDto inputDto)
        {
            var output = new Response<GetAddOnOutputDto>();

            try
            {
                //Input Validation
                var validator = new CreateAddOnInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                // => business validation.
                //check on (name and title)
                if (IsAddonNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(CreateAddOnInputDto.Name));
                if (IsAddonNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(CreateAddOnInputDto.Name));
                if (IsAddonTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(CreateAddOnInputDto.Title));
                if (IsAddonTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(CreateAddOnInputDto.Title));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                // upload image.
                var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                if (createdFileResult.Errors.Any())
                {
                    return output.AppendErrors(createdFileResult.Errors).CreateResponse();
                }

                var addonModel = _mapper.Map<AddOn>(inputDto);

                //addonModel.CreatedBy = inputDto.CreatedBy;
                addonModel.Logo = createdFileResult.Data;

                _addonRepository.Add(addonModel);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetAddOnOutputDto>(addonModel);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetAddOnOutputDto>> UpdateAsync(UpdateAddOnInputDto inputDto)
        {
            var output = new Response<GetAddOnOutputDto>();

            try
            {

                //Input Validation
                var validator = new UpdateAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //TODO:Try to Refactor this
                FileStorage file = null;
                if (inputDto.File != null)
                {
                    // upload image.
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.ModifiedBy.GetValueOrDefault(0));
                    file = createdFileResult.Data;
                }

                //Business Validations
                // 1- Check if Already Exists  (Name)
                if (IsAddonNameExist(GetJsonLanguageModelOrNull(inputDto?.Name), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(AddOn.Name));
                if (IsAddonNameExist(GetJsonLanguageModelOrNull(inputDto?.Name), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(AddOn.Name));
                // 2- Check if Already Exists  (Title)
                if (IsAddonTitleExist(GetJsonLanguageModelOrNull(inputDto?.Title), LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(AddOn.Title));
                if (IsAddonTitleExist(GetJsonLanguageModelOrNull(inputDto?.Title), LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(AddOn.Title));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    output.AppendError(MessageCodes.NotFound, nameof(AddOn));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //update AddOn
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Logo = file;
                entity = _addonRepository.Update(entity);
                _unitOfWork.Commit();

                var result = _mapper.Map<GetAddOnOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> DeleteAsync(DeleteTrackedEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var entity = _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnLabel));

                // 1- Check if Entity has references
                var checkDto = EntityHasReferences(inputDto.Id, _addonRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(AddOnLabel)));
                if (checkDto.HasChildren == 0)
                {
                    //hard delete related entity
                    _addonLabelRepository.Delete(x => x.AddOnId == inputDto.Id);

                    //soft delete entity
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;

                    _unitOfWork.Commit();
                    return await Task.FromResult(output.CreateResponse(true));
                }

                else
                {
                    //reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }


        }

        /// <summary>
        /// Get  List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<List<GetAddOnOutputDto>>> GetAllAsync()
        {
            var output = new Response<List<GetAddOnOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetAddOnOutputDto>>(_addonRepository.GetAllList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetAddOnOutputDto>> GetByIdAsync(GetAddOnInputDto inputDto)
        {
            var output = new Response<GetAddOnOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetAddOnOutputDto>(entity);

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        ///// <summary>
        ///// Get Paged List of only Active and not Deleted records
        ///// </summary>
        ///// <param name="pagedDto"></param>
        ///// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetAddOnOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetAddOnOutputDto>>();

            var result = GetPagedList<GetAddOnOutputDto, AddOn, int>(
                pagedDto: pagedDto,
                repository: _addonRepository,
                orderExpression: x => x.Id,
                searchExpression: x => string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.Name.Contains(pagedDto.SearchTerm)
                      || x.Title.Contains(pagedDto.SearchTerm)
                      || x.MainPageUrl.Contains(pagedDto.SearchTerm)
                      || x.ShortDescription.Contains(pagedDto.SearchTerm))),
               sortDirection: pagedDto.SortingDirection,
                      disableFilter: true);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<AddonDetailsDto>> GetAddonDetailsAsync(int addonId, int? currentUserId = null)
        {
            var response = new Response<AddonDetailsDto>();

            var addon = await _addonRepository.GetByIdAsync(addonId);

            if (addon is null)
            {
                response.CreateResponse(MessageCodes.NotFound, nameof(AddOn));
                return response;
            }

            var addonDetails = _mapper.Map<AddonDetailsDto>(addon);


            if ((currentUserId != null || currentUserId != 0) && addon.WishListAddOns.Any(w => w.CustomerId == currentUserId))
                addonDetails.IsInWishlist = true;

            var versions = addon.VersionAddons.Where(x => x.Version != null && x.Version.VersionPrices.Any() && x.Version.VersionReleases.Any() && x.Version.Application != null).Select(v => v.Version).ToList();

            foreach (var version in versions)
            {
                if (version != null)
                {

                    var versionTitle = JsonConvert.DeserializeObject<JsonLanguageModel>(version.Title);
                    var appTitle = JsonConvert.DeserializeObject<JsonLanguageModel>(version.Application.Title);

                    var concatenatedAppVersionTitle = new JsonLanguageModel
                    {
                        Default = $"{appTitle.Default} {versionTitle.Default}",
                        Ar = $"{appTitle.Ar} {versionTitle.Ar}"
                    };

                    var title = JsonConvert.SerializeObject(concatenatedAppVersionTitle);

                    addonDetails.AvailableAppVersions.Add(new AppVersionDto
                    {
                        Id = version.Id,
                        ApplicationId = version.ApplicationId,
                        VersionReleaseId = version.VersionReleases.Where(r => r.IsCurrent).FirstOrDefault()?.Id ?? 0,
                        SubscribtionTypeId = version.Application.SubscriptionTypeId,
                        Logo = _mapper.Map<FileStorageDto>(version.Image)/*.FullPath*/,
                        Title = title
                    });
                }
            }


            var addonPrices = _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                             .Where(ap => ap.AddOnId == addonId);

            var versionPrices = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                             .Where(vp => addonPrices.Select(x => x.PriceLevelId).Contains(vp.PriceLevelId));



            if (currentUserId != null && currentUserId > 0)
            {
                var purshasedversions = _versionSubscriptionRepository.Where(x => x.CustomerSubscription.Invoices
                                                                                                                 .Any(v => v.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                               && x.CustomerSubscription.CustomerId == currentUserId && versionPrices.Select(x => x.Id).Contains(x.VersionPriceId) &&
                               !x.AddonSubscriptions.Where(e => !e.CustomerSubscription.Invoices.Any(e => e.InvoiceStatusId == (int)InvoiceStatusEnum.Cancelled))
                               .Any(e => e.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid &&
                                                       e.AddonPrice.AddOnId == addonId)))
                               .AsEnumerable()
                               .Select(vs =>
                               {
                                   var versionPrice = versionPrices.Where(vp => vp.Id == vs.VersionPriceId).FirstOrDefault();

                                   if (versionPrice != null)
                                   {
                                       var version = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(v => v.Id == versionPrice.VersionId);
                                       var priceLevel = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(pl => pl.Id == versionPrice.PriceLevelId);
                                       var application = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(a => a.Id == version.ApplicationId);
                                       var verionAddon = _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                                                           .FirstOrDefault(va => va.VersionId == version.Id);

                                       return new PurchasedVersionsDto
                                       {
                                           ApplicationId = application.Id,
                                           ApplicationName = application.Name,
                                           LicensesCount = priceLevel.NumberOfLicenses,
                                           PriceLevelId = priceLevel.Id,
                                           PriceLevelName = priceLevel.Name,
                                           VersionId = version.Id,
                                           VersionName = version.Name,
                                           VersionPriceId = versionPrice.Id,
                                           VersionAddOnId = verionAddon.Id,
                                           VersionReleaseId = vs.VersionReleaseId,
                                           Subscription = _mapper.Map<GetSubscriptionTypeOutputDto>(application.SubscriptionType),
                                           VersionSubscription = vs,
                                           NextRenewDate = vs.CustomerSubscription.Invoices
                                                   .Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                                   .OrderByDescending(i => i.Id)
                                                   .FirstOrDefault()?.CreateDate.AddDays(vs.CustomerSubscription.RenewEvery) ?? DateTime.UtcNow,
                                           UsedLicensesCount = vs.CustomerSubscription.Licenses.Count,
                                           Discrimnator = GetInvoiceDiscriminator(application.SubscriptionTypeId, vs.CustomerSubscription.RenewEvery),
                                       };
                                   }
                                   return null;
                               }).ToList();



                purshasedversions.RemoveAll(x => x.VersionSubscriptionId == 0);
                purshasedversions.ForEach(x => x.AddOnPriceId = _addOnPriceRepository.Where(a => a.AddOnId == addonId && a.PriceLevelId == x.PriceLevelId).Select(a => a.Id).FirstOrDefault());

                addonDetails.PurchasedVersions.AddRange(purshasedversions);

            }

            response.CreateResponse(addonDetails);
            return response;
        }

        public DateTime GetVersionNextRenewDate(VersionSubscription versionSubscription)
        {
            return versionSubscription.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)
                                ? versionSubscription.CustomerSubscription.Invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid).OrderByDescending(i => i.Id).FirstOrDefault().EndDate/*StartDate.AddDays(versionSubscription.CustomerSubscription.RenewEvery)*/
                                : DateTime.UtcNow;
        }
        public AddOnForBuyNowOutputDto GetAddOnDataForBuyNow(int addOnId, int priceLevelId, int versionId = 0)
        {
            var output = new AddOnForBuyNowOutputDto();

            try
            {

                //Business Validation
                var addOn = _addonRepository
                    .WhereIf(x => x.VersionAddons.Any(v => v.VersionId == versionId), versionId > 0)
                    .Where(x => x.Id == addOnId)
                    .FirstOrDefault();

                if (addOn == null)
                    return null;

                if (priceLevelId > 0)
                {
                    var priceLevel = _priceLevelRepository.Where(x => x.Id == priceLevelId).FirstOrDefault();
                    if (priceLevel == null)
                        return null;
                }

                var mappedResult = _mapper.Map<GetAddOnDataOutputDto>(addOn);
                //remove addOnprices that not exist with pricelevelid
                if (priceLevelId > 0)
                    mappedResult.AddOnPrices.RemoveAll(vp => priceLevelId != vp.PriceLevelId);
                mappedResult.MinAddOnPrice = new AddOnPriceDto { AddOnId = mappedResult.Id, PriceLevelId = priceLevelId };
                return _mapper.Map<AddOnForBuyNowOutputDto>(mappedResult);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int GetMissingPriceCount(int addonId, IEnumerable<int> countryCurrencyIds)
        {
            return _missingPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == addonId && countryCurrencyIds.Contains(x.CountryCurrencyId)).Count();
        }
        #endregion

        #region AddonTags
        public IResponse<bool> AddAddonTag(CreateAddonTagInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {

                //Input Validation Validations
                var validator = new CreateAddonTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-check entity already exists
                if (IsAddonTagExsistedBefore(inputDto))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOnTag));
                //Business
                var mappedInput = _mapper.Map<AddOnTag>(inputDto);

                //check if this is the first record => set Featured = true;
                var firstRecord = !(_addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == inputDto.AddonId).Any());
                if (firstRecord)
                    mappedInput.IsFeatured = true;

                _addontagRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(true);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public IResponse<bool> AssignFeaturedToAddonTag(int Id)
        {
            var output = new Response<bool>();

            try
            {
                //Business
                var entity = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == Id);
                // Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnTag));

                //update All other AddonTags to not featured 
                UpdateOldFeaturedAddonTagToNotFeatured(entity.AddOnId);

                entity.IsFeatured = true;
                //update AddonTag 
                entity = _addontagRepository.Update(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public IResponse<List<GetTagOutputDto>> GetAllActiveTagsNotAssignToAddonId(int AddonId)
        {
            var output = new Response<List<GetTagOutputDto>>();
            try
            {
                var addOnEntity = _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == AddonId);
                // 1- Check if Entity Exists
                if (addOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                var Addontags = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == AddonId).Select(f => f.TagId).ToList();
                var result = _mapper.Map<List<GetTagOutputDto>>(_tagRepository.Where(d => !Addontags.Contains(d.Id)));
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<GetAddonTagOutputDto> UpdateAddonTag(UpdateAddonTagDto inputDto)
        {
            var output = new Response<GetAddonTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateAddonTagDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Tag Assigned to this AddOn Before
                if (IsAddonTagExsistedBefore(inputDto, inputDto.Id))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOnTag));

                //Business
                var entity = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnTag));
                //3-check if try to update IsFeatured to Not IsFeatured
                if (entity.IsFeatured && !inputDto.IsFeatured)
                {
                    //assign IsFeatured to the first entity
                    var FirstEntity = GetFirstAddOnTag(entity.AddOnId);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                    //reject to update
                    else
                        return output.CreateResponse(MessageCodes.BusinessValidationError);
                }

                entity = _mapper.Map(inputDto, entity);
                //update AddonTag 
                entity = _addontagRepository.Update(entity);
                _unitOfWork.Commit();

                var result = _mapper.Map<GetAddonTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public IResponse<bool> DeleteAddonTag(DeleteEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business
                var entity = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                //1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnTag));

                //2-check if deleted Entity is Featured => Assign IsFeatured to The first AddonTag 
                if (entity.IsFeatured)
                {
                    var FirstEntity = GetFirstAddOnTag(entity.AddOnId, entity.Id);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                }

                //3-check if entity has related data
                var checkDto = EntityHasReferences(entity.Id, _addontagRepository);
                if (checkDto.HasChildren == 0)
                {
                    //Hard Delete AddonTag 
                    _addontagRepository.Delete(entity);
                    _unitOfWork.Commit();
                    return output.CreateResponse(true);
                }
                else
                {
                    //reject delete if entity has references in any other tables
                    return output.CreateResponse(MessageCodes.RelatedDataExist);

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetAddonTagOutputDto>> GetAddOnTagByIdAsync(GetAddOnTagInputDto inputDto)
        {
            var output = new Response<GetAddonTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetAddOnTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetAddonTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<List<GetAddonTagOutputDto>> GetAllAddOnTagListByAddonId(int AddonId)
        {
            var output = new Response<List<GetAddonTagOutputDto>>();
            try
            {   //Business
                var addOnEntity = _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == AddonId);
                // 1- Check if Entity Exists
                if (addOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                var result = _mapper.Map<List<GetAddonTagOutputDto>>(_addontagRepository.Where(C => C.AddOnId == AddonId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<PagedResultDto<GetAddonTagOutputDto>>> GetAddOnTagPagedListAsync(AddOnFilteredPagedResult inputDto)
        {
            var output = new Response<PagedResultDto<GetAddonTagOutputDto>>();

            var result = GetPagedList<GetAddonTagOutputDto, AddOnTag, int>(
                pagedDto: inputDto,
                repository: _addontagRepository,
                orderExpression: x => x.Id,
                searchExpression: x => x.AddOnId == inputDto.AddOnId && (string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                    || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm) && (x.Tag.Name.Contains(inputDto.SearchTerm)))),
                sortDirection: inputDto.SortingDirection,
                disableFilter: true,
                 excluededColumns: null,
                 x => x.Tag, x => x.AddOn
                  );
            return output.CreateResponse(result);

        }



        #endregion

        #region AddonLabels

        public IResponse<GetAddonLabelOutputDto> CreateAddonLabel(CreateAddonLabelInputDto inputDto)
        {
            var output = new Response<GetAddonLabelOutputDto>();

            try
            {

                //Input Validation Validations
                var validator = new AddAddonLabelValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //check if Label Name  Length (Arabic and English) not exists before
                if (IsAddonLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.AddOnId, LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Name));
                if (IsAddonLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.AddOnId, LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business Validations
                //1-check if Addon Has Label Before 
                if (_addonLabelRepository.Any(s => s.AddOnId == inputDto.AddOnId))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOnLabel));

                //Business
                var mappedInput = _mapper.Map<AddOnLabel>(inputDto);

                _addonLabelRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetAddonLabelOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public IResponse<GetAddonLabelOutputDto> UpdateAddonLabel(UpdateAddonLabelInputDto inputDto)
        {
            var output = new Response<GetAddonLabelOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateAddonLabelDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //Business
                var entity = _addonLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.AddOnId == inputDto.AddOnId);
                // 1-Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                //2-Check if Label Name  Length (Arabic and English) not exists before
                if (IsAddonLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.AddOnId, LangEnum.Default, entity.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(AddOnLabel.Name));
                if (IsAddonLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.AddOnId, LangEnum.Ar, entity.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(AddOnLabel.Name));


                if (!output.IsSuccess)
                    return output.CreateResponse();

                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Tag
                entity = _addonLabelRepository.Update(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetAddonLabelOutputDto>(entity));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<bool> DeleteAddonLabel(DeleteTrackedEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business
                var entity = _addonLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.AddOnId == inputDto.Id);

                // 1-Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                //2-check if entity has related data
                var checkDto = EntityHasReferences(entity.Id, _addonLabelRepository);
                if (checkDto.HasChildren == 0)
                {
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _addonLabelRepository.Update(entity);

                    _unitOfWork.Commit();
                    return output.CreateResponse(true);
                }
                else
                {
                    //reject delete if entity has references in any other tables
                    return output.CreateResponse(MessageCodes.RelatedDataExist);

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }


        public async Task<IResponse<GetAddonLabelOutputDto>> GetAddOnLabelByAddOnIdAsync(GetAddOnLabelInputDto inputDto)
        {
            var output = new Response<GetAddonLabelOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetAddOnLabelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _addonLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.AddOnId == inputDto.AddOnId);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetAddonLabelOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public IResponse<List<GetAddonLabelOutputDto>> GetAllAddOnLabelListByAddonId(GetAddOnLabelInputDto inputDto)
        {
            var output = new Response<List<GetAddonLabelOutputDto>>();
            try
            {
                //Input Validation
                var validator = new GetAddOnLabelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var result = _mapper.Map<List<GetAddonLabelOutputDto>>(_addonLabelRepository.Where(C => C.AddOnId == inputDto.AddOnId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<PagedResultDto<GetAddonLabelOutputDto>>> GetAddOnLabelPagedListAsync(AddOnFilteredPagedResult inputDto)
        {
            var output = new Response<PagedResultDto<GetAddonLabelOutputDto>>();

            var result = GetPagedList<GetAddonLabelOutputDto, AddOnLabel, int>(
                pagedDto: inputDto,
                repository: _addonLabelRepository,
                orderExpression: x => x.Id,
                searchExpression: x => x.AddOnId == inputDto.AddOnId && (string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm) &&
                         ((x.Name.Contains(inputDto.SearchTerm)
                      || x.Color.Contains(inputDto.SearchTerm)
                      || x.AddOn.Name.Contains(inputDto.SearchTerm))))),
                sortDirection: inputDto.SortingDirection,
               disableFilter: true);
            return output.CreateResponse(result);

        }

        #endregion

        #region AddonPrices
        public async Task<IResponse<GetAddOnPriceOutputDto>> CreateAddOnPriceAsync(CreateAddOnPriceInputDto inputDto)
        {
            var output = new Response<GetAddOnPriceOutputDto>();
            try
            {
                //Input Validation Validations
                var validator = new CreateAddOnPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                //1-check entity already exists
                if (IsAddOnPriceAlreadExsist(inputDto))
                {
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOnPrice));
                }
                //Business
                var entity = _mapper.Map<AddOnPrice>(inputDto);
                await _addOnPriceRepository.AddAsync(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetAddOnPriceOutputDto>(entity);
                //entity = _addOnPriceRepository.FindByInclude(x=> x.Id == entity
                //.Id,x => x.PriceLevel).FirstOrDefault();
                return output.CreateResponse(result);

                //return output.WithData(result).CreateResponse();

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);


            }
        }

        public async Task<IResponse<GetAddOnPriceOutputDto>> UpdateAddOnPriceAsync(UpdateAddOnPriceInputDto inputDto)
        {
            var output = new Response<GetAddOnPriceOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateAddOnPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists AddOnPrice 
                if (IsAddOnPriceAlreadExsist(inputDto, inputDto.Id))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOnPrice));


                //Business
                var entity = _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnPrice));


                entity = _mapper.Map(inputDto, entity);
                //update AddOnPrice
                entity = _addOnPriceRepository.Update(entity);
                _unitOfWork.Commit();

                var result = _mapper.Map<GetAddOnPriceOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> DeleteAddOnPriceAsync(DeleteTrackedEntityInputDto inputDto)
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var entity = _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOnPrice));

                var checkDto = EntityHasReferences(inputDto.Id, _addOnPriceRepository);
                if (checkDto.HasChildren == 0)
                {
                    //soft delete entity
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _unitOfWork.Commit();
                    return await Task.FromResult(output.CreateResponse(true));
                }

                else
                {
                    //reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

                }
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }


        }

        /// <summary>
        /// Get  List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<List<GetAddOnPriceOutputDto>>> GetAllExistingAddOnPricesAsync(GetAddOnPricesInputDto inputDto)
        {
            var output = new Response<List<GetAddOnPriceOutputDto>>();
            try
            {
                var validator = new GetAddOnPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var result = _mapper.Map<List<GetAddOnPriceOutputDto>>(_addOnPriceRepository/*.DisableFilter(nameof(DynamicFilters.IsActive))*/.Where(x => x.AddOn != null && x.AddOnId == inputDto.AddOnId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<GetAddOnPriceOutputDto>>> GetAllMissingAddOnPricesAsync(GetAddOnPricesInputDto inputDto)
        {
            var output = new Response<List<GetAddOnPriceOutputDto>>();
            try
            {
                var validator = new GetAddOnPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var result = _missingPriceRepository.Where(x => x.AddOnId == inputDto.AddOnId).ToList();
                var missingAddOnPrices = _mapper.Map<List<GetAddOnPriceOutputDto>>(result);
                return output.CreateResponse(missingAddOnPrices);
                #region Old EF
                ////get all expected Combinations of (countryIds,priceLevelIds) for  AddOnPrices and exclude Existing database records of them
                //var countryCurrencies = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).ToList();
                //var priceLevels = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).ToList();
                //var allAddOnPrices = countryCurrencies.SelectMany(x => priceLevels, ( c, p ) => new GetAddOnPriceOutputDto
                //{
                //    Id = 0,
                //    AddOnId = inputDto.AddOnId,
                //    CountryCurrencyId = c.Id,
                //    CountryName = c.Country.Name,
                //    CurrencyShortCode = c.Currency.Symbole,
                //    PriceLevelId = p.Id,
                //    PriceLevelName = p.Name,
                //    NumberOfLicenses = p.NumberOfLicenses.ToString(),
                //    IsActive = true
                //}).ToList();

                //var existingAddOnPrices = _mapper.Map<List<GetAddOnPriceOutputDto>>(_addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == inputDto.AddOnId).ToList());
                //var missingAddOnPrices = allAddOnPrices.Except(existingAddOnPrices, new AddOnPricesEqualityComparer()).ToList();

                //return output.CreateResponse(missingAddOnPrices);
                #endregion
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetAddOnPriceOutputDto>> GetAddOnPriceByIdAsync(GetAddOnPriceInputDto inputDto)
        {
            var output = new Response<GetAddOnPriceOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetAddOnPriceInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == inputDto.Id).FirstOrDefault();
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetAddOnPriceOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        /// <summary>
        /// Get Paged List of only Active and not Deleted records
        /// </summary>
        /// <param name="pagedDto"></param>
        /// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetAddOnPriceOutputDto>>> GetAllExistingAddOnPricePagedListAsync(AddOnFilteredPagedResult pagedDto, int currentEmployeeId)
        {
            var output = new Response<PagedResultDto<GetAddOnPriceOutputDto>>();

            var countryCurrencyIds = _employeeBLL.GetEmployeeCountryCurrencies(currentEmployeeId);
            //var activeAddOnIds = _addonRepository.GetAll().Select(x=>x.Id).ToList();

            var result = GetPagedList<GetAddOnPriceOutputDto, AddOnPrice, int>(
                pagedDto: pagedDto,
                repository: _addOnPriceRepository,
                orderExpression: x => x.Id,
                searchExpression: x =>/* activeAddOnIds.Contains(x.AddOnId) && */x.AddOnId == pagedDto.AddOnId && countryCurrencyIds.Contains(x.CountryCurrencyId) && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         ((x.CountryCurrency.Country.Name.Contains(pagedDto.SearchTerm)
                      || x.CountryCurrency.Currency.Symbole.Contains(pagedDto.SearchTerm)
                      || x.PriceLevel.Name.Contains(pagedDto.SearchTerm))))),
                sortDirection: pagedDto.SortingDirection,
               disableFilter: true,
               excluededColumns: null,
              x => x.AddOn, x => x.CountryCurrency, x => x.PriceLevel);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<PagedResultDto<GetAddOnPriceOutputDto>>> GetAllMissingAddOnPricesPagedListAsync(AddOnFilteredPagedResult pagedDto, int currentEmployeeId)
        {
            var output = new Response<PagedResultDto<GetAddOnPriceOutputDto>>();

            try
            {
                var countryCurrencyIds = _employeeBLL.GetEmployeeCountryCurrencies(currentEmployeeId);

                var result = GetPagedList<GetAddOnPriceOutputDto, ViewMissingAddOnPrice, int>(pagedDto: pagedDto, repository: _missingPriceRepository, orderExpression: x => x.AddOnId,
                      searchExpression: x => x.AddOnId == pagedDto.AddOnId && countryCurrencyIds.Contains(x.CountryCurrencyId) && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.CountryName.Contains(pagedDto.SearchTerm)
                      || x.CurrencyShortCode.Contains(pagedDto.SearchTerm)
                      || x.PriceLevelName.Contains(pagedDto.SearchTerm)))),
                  sortDirection: pagedDto.SortingDirection,
                  disableFilter: true);

                return output.CreateResponse(result);

                #region Old EF
                ////get all expected Combinations of (countryIds,priceLevelIds) for  AddOnPrices and exclude Existing database records of them
                //var countryCurrencies = _countryCurrencyRepository.DisableFilter(nameof(DynamicFilters.IsActive)).ToList();
                //var priceLevels = _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).ToList();
                //var allAddOnPrices = countryCurrencies.SelectMany(x => priceLevels, ( c, p ) => new GetAddOnPriceOutputDto
                //{
                //    Id = 0,
                //    AddOnId = inputDto.AddOnId,
                //    CountryCurrencyId = c.Id,
                //    CountryName = c.Country.Name,
                //    CurrencyShortCode = c.Currency.Symbole,
                //    PriceLevelId = p.Id,
                //    PriceLevelName = p.Name,
                //    NumberOfLicenses = p.NumberOfLicenses.ToString(),
                //    IsActive = true
                //}).ToList();

                //var existingAddOnPrices = _mapper.Map<List<GetAddOnPriceOutputDto>>(_addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == inputDto.AddOnId).ToList());
                //var missingAddOnPrices = allAddOnPrices.Except(existingAddOnPrices, new AddOnPricesEqualityComparer()).ToList();

                ////TODO:Search & Filter
                //PagedResultDto<GetAddOnPriceOutputDto> result = new PagedResultDto<GetAddOnPriceOutputDto>
                //{
                //    TotalCount = missingAddOnPrices.Count(),
                //    Items = missingAddOnPrices.Skip(inputDto.SkipCount).Take(inputDto.MaxResultCount).ToList()
                //};

                //return output.CreateResponse(result);
                #endregion
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        private AddOnPrice GetMinimumAddonPriceDb(int addonId, int? countryId = null)
        {
            var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;

            var pricesList = _addOnPriceRepository
                                    .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                                    .Where(ap => ap.AddOnId == addonId)
                                    .ToList();


            return pricesList.Aggregate((curMin, x) =>
            {
                decimal curMinValue = Math.Min(x.MonthlyNetPrice, Math.Min(x.YearlyNetPrice, x.ForeverNetPrice));
                decimal minValue = Math.Min(curMin.MonthlyNetPrice, Math.Min(curMin.YearlyNetPrice, curMin.ForeverNetPrice));

                return curMinValue < minValue ? x : curMin;
            });
        }
        public VersionPriceAllDetailsDto GetMinimumAddonPrice_(int addonId, int? countryId = null)
        {
            try
            {

                var min = GetMinimumAddonPriceDb(addonId, countryId);




                return new VersionPriceAllDetailsDto
                {
                    Forever = _mapper.Map<ForeverPriceDetailsDto>(min),
                    Monthly = _mapper.Map<MonthlyPriceDetailsDto>(min),
                    Yearly = _mapper.Map<YearlyPriceDetailsDto>(min),
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public AddonPriceDetailsDto GetMinimumAddonPrice(int addonId, int? countryId = null)
        {
            try
            {




                //var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;

                //var pricesList = _addOnPriceRepository
                //                        .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                //                        .Where(ap => ap.AddOnId == addonId)
                //                        .ToList();


                var min = GetMinimumAddonPriceDb(addonId, countryId);
                //decimal minValue = decimal.MaxValue;
                //decimal currentValue = 0;

                //var min = pricesList.Aggregate((curMin, x) =>
                //{

                //    currentValue = Math.Min(x.MonthlyNetPrice, Math.Min(x.YearlyNetPrice, x.ForeverNetPrice));
                //    if (currentValue < minValue || currentValue == 0)
                //    {
                //        minValue = currentValue;
                //        curMin = x;
                //    }
                //    return curMin;
                //});



                #region Comment
                //var minimumPrice = _addOnPriceRepository
                //                        .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                //                        .Where(ap => ap.AddOnId == addonId && ap.MonthlyNetPrice > default(int))
                //                        .OrderBy(ap => ap.MonthlyNetPrice)
                //                        .FirstOrDefault();

                //var pricesList = _addOnPriceRepository
                //                        .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                //                        .Where(ap => ap.AddOnId == addonId)
                //                        .ToList();
                //var minMonthlyPrice = pricesList.OrderBy(ap => ap.MonthlyNetPrice).FirstOrDefault();
                //var minYearlPrice = pricesList.OrderBy(ap => ap.YearlyNetPrice).FirstOrDefault();
                //var minForeverPrice = pricesList.OrderBy(ap => ap.ForeverNetPrice).FirstOrDefault();
                //var minPrice = _addOnPriceRepository
                //       .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                //       .GroupBy(x => x.AddOnId)
                //       .Select(x => new AddOnMinPriceDto
                //       {
                //           MinMonthlyPrice = x.Min(m => m.MonthlyNetPrice),
                //           MinYearylyPrice = x.Min(m => m.YearlyNetPrice),
                //           MinForEverPrice = x.Min(m => m.ForeverNetPrice)

                //       })
                //       .OrderBy(x => x.MinPrice)
                //       .FirstOrDefault();
                #endregion
                return _mapper.Map<AddonPriceDetailsDto>(min);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IResponse<AddOnPriceData>> GetAddonPriceByPriceLevel(int addOnId, int? countryId = null, int? priceLevelId = null)
        {
            var output = new Response<AddOnPriceData>();

            try
            {
                AddOnPriceData result = new AddOnPriceData();
                var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;
                var pricesList = _addOnPriceRepository
                                        .WhereIf(ap => ap.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                                        .WhereIf(vp => vp.PriceLevelId == priceLevelId, priceLevelId.HasValue && priceLevelId.Value > 0)
                                        .Where(ap => ap.AddOnId == addOnId)
                                        .ToList();
                decimal minValue = 0;
                decimal currentValue = 0;
                if (pricesList != null && pricesList.Count() > 0)
                {
                    var min = pricesList.Aggregate((curMin, x) =>
                      {
                          currentValue = Math.Min(x.MonthlyNetPrice, Math.Min(x.YearlyNetPrice, x.ForeverNetPrice));
                          if (currentValue < minValue || currentValue == 0)
                          {
                              minValue = currentValue;
                              curMin = x;
                          }
                          return curMin;
                      });
                    result.Price = new AddOnPriceAllDetailsDto
                    {

                        PriceLevelId = min.PriceLevelId,
                        AddOnPriceId = _addOnPriceRepository.Where(x => x.AddOnId == addOnId && x.PriceLevelId == min.PriceLevelId).Select(x => x.Id).FirstOrDefault(),
                        Forever = _mapper.Map<ForeverPriceDetailsDto>(min),
                        Monthly = _mapper.Map<MonthlyPriceDetailsDto>(min),
                        Yearly = _mapper.Map<YearlyPriceDetailsDto>(min),
                    };
                }
                return output.CreateResponse(result);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

        #region Sliders.
        public async Task<IResponse<bool>> AddAddoneSlider(FileDto fileDto, NewAddonSliderDto newAddonSliderDto, int currentUserId)
        {
            var result = new Response<bool>();

            try
            {
                var fileValidator = new FileDtoValidator().Validate(fileDto);

                if (!fileValidator.IsValid)
                {
                    return result.CreateResponse(fileValidator.Errors);
                }

                var validator = new NewAddonSliderDtoValidator().Validate(newAddonSliderDto);

                if (!validator.IsValid)
                {
                    return result.CreateResponse(validator.Errors);
                }
                var file = new SingleFilebaseDto
                {
                    FileDto = fileDto
                };

                var fileResult = await _BlobfileBLL.UploadFileAsync(file);
                //var fileResult = _fileBLL.UploadFile(fileDto, currentUserId);

                if (!fileResult.IsSuccess)
                {
                    result.AppendErrors(fileResult.Errors);

                    return result.CreateResponse();
                }

                var sliderModel = _mapper.Map<AddOnSlider>(newAddonSliderDto);

                sliderModel.CreatedBy = currentUserId;
                sliderModel.Media = fileResult.Data;

                _addonSliderRepository.Add(sliderModel);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                return result.CreateResponse(ex);
            }

            return result.CreateResponse(true);
        }

        public IResponse<PagedResultDto<RetrieveAddonSliderDto>> GetAllAddonSlidersPagedList(AddOnFilteredPagedResult pagedDto)
        {
            var result = new Response<PagedResultDto<RetrieveAddonSliderDto>>();

            try
            {
                var slidersDto = GetPagedList<RetrieveAddonSliderDto, AddOnSlider, int>(pagedDto: pagedDto,
                                                                                        repository: _addonSliderRepository,
                                                                                        orderExpression: s => s.Id,
                                                                                        searchExpression: s => s.AddOnId == pagedDto.AddOnId,
                                                                                        sortDirection: pagedDto.SortingDirection,
                                                                                        disableFilter: true);
                return result.CreateResponse(slidersDto);
            }
            catch (Exception ex)
            {
                return result.CreateResponse(ex);
            }
        }

        public IResponse<bool> DeleteAddonSlider(DeleteTrackedEntityInputDto inputDto)
        {
            var result = new Response<bool>();

            try
            {

                //Input Validation
                var validator = new DeleteTrackedEntityInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return result.CreateResponse(validator.Errors);
                }

                var sliderModel = _addonSliderRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(s => s.Id == inputDto.Id);

                if (sliderModel is null)
                    return result.CreateResponse(MessageCodes.NotFound, nameof(AddOnSlider));

                //check if entity has related data
                var checkDto = EntityHasReferences(sliderModel.Id, _addonSliderRepository);
                if (checkDto.HasChildren == 0)
                {
                    sliderModel.IsDeleted = true;
                    sliderModel.ModifiedBy = inputDto.ModifiedBy;
                    sliderModel.ModifiedDate = inputDto.ModifiedDate;
                    _addonSliderRepository.Update(sliderModel);
                    _unitOfWork.Commit();
                    return result.CreateResponse(true);
                }
                else
                {
                    //reject delete if entity has references in any other tables
                    return result.CreateResponse(MessageCodes.RelatedDataExist);

                }
            }
            catch (Exception ex)
            {
                result.CreateResponse(ex);
            }

            return result.CreateResponse(true);
        }
        #endregion

        #region Helpers


        public bool IsAddOnPriceAlreadExsist(CreateAddOnPriceInputDto dto, int? Id = null)
        {
            return _addOnPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(pl => pl.Id != Id, Id.HasValue).Any(s => s.AddOnId == dto.AddOnId && s.CountryCurrencyId == dto.CountryCurrencyId && s.PriceLevelId == dto.PriceLevelId);
        }
        public bool IsAddonTagExsistedBefore(AddonTagDto dto, int? Id = null)
        {
            return _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(pl => pl.Id != Id, Id.HasValue).Any(s => s.AddOnId == dto.AddonId && s.TagId == dto.TagId);
        }
        private List<AddOnTag> UpdateOldFeaturedAddonTagToNotFeatured(int addOnId)
        {
            var oldFeatured = _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddOnId == addOnId && x.IsFeatured).ToList();
            oldFeatured.ForEach(c =>
            {
                c.IsFeatured = false;
                _addontagRepository.Update(c);

            });
            return oldFeatured;
        }
        private AddOnTag GetFirstAddOnTag(int addOnId, int? addOnTagId = null)
        {
            return _addontagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != addOnTagId, addOnTagId.HasValue).Where(f => f.AddOnId == addOnId).OrderBy(f => f.Id).FirstOrDefault();
        }
        private int GetInvoiceDiscriminator(int subscribtionTypeId, int renewEvery)
        {
            if (subscribtionTypeId != (int)InvoiceTypes.Renewal) //this is for for support and forever subscription
                return (int)DiscriminatorsEnum.Forever;
            else if (subscribtionTypeId == (int)InvoiceTypes.Renewal && renewEvery == 30)
                return (int)DiscriminatorsEnum.Monthly;
            else if (subscribtionTypeId == (int)InvoiceTypes.Renewal && renewEvery == 365)
                return (int)DiscriminatorsEnum.Yearly;
            return 0;

        }

        public async Task<DownloadPriceDto> GetMimumPriceWithCurrency(AddOn addOn, int? currencyId)
        {
            try
            {
                var addOns = await _addonRepository.Where(v => v.Id == addOn.Id).ToListAsync();
                var monthlyPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.MonthlyNetPrice).DefaultIfEmpty(default).Min();
                var yearlyPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.YearlyNetPrice).DefaultIfEmpty(default).Min();
                var foreverPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.ForeverNetPrice).DefaultIfEmpty(default).Min();
                return new DownloadPriceDto
                {
                    MonthlyPrice = monthlyPrice ?? default,
                    YearlyPrice = yearlyPrice ?? default,
                    ForeverPrice = foreverPrice ?? default
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public async Task<DownloadPriceDto> GetMimumPriceWithCurrency(AddOn addOn, int? currencyId)
        //{
        //    try
        //    {
        //        var addOns = await _addonRepository.Where(v => v.Id == addOn.Id).ToListAsync();
        //        var monthlyPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.MonthlyNetPrice).DefaultIfEmpty(default).Min();
        //        var yearlyPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.YearlyNetPrice).DefaultIfEmpty(default).Min();
        //        var foreverPrice = addOns?.SelectMany(v => v.AddOnPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.ForeverNetPrice).DefaultIfEmpty(default).Min();
        //        return new DownloadPriceDto
        //        {
        //            MonthlyPrice = monthlyPrice ?? default,
        //            YearlyPrice = yearlyPrice ?? default,
        //            ForeverPrice = foreverPrice ?? default
        //        };
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        #region Json

        private bool IsAddonNameExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? addonId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                     ? _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim())
                                    .WhereIf(a => a.Id != addonId, addonId.HasValue).Any()
                     : _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim())
                                    .WhereIf(a => a.Id != addonId, addonId.HasValue).Any();
        }
        private bool IsAddonTitleExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? addonId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
            ? _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.default") == name.Default.Trim())
                           .WhereIf(a => a.Id != addonId, addonId.HasValue).Any()
            : _addonRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.ar") == name.Ar.Trim())
                           .WhereIf(a => a.Id != addonId, addonId.HasValue).Any();
        }
        private bool IsAddonLabelNameExist(JsonLanguageModel name, int addonId, LangEnum lang = LangEnum.Default, int? excludedId = null)
        {


            return lang == LangEnum.Default
              ? _addonLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim() && a.AddOnId == addonId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any()
              : _addonLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim() && a.AddOnId == addonId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any();
        }


        #endregion
        #endregion
    }
}
