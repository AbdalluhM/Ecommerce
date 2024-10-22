using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Applications.Versions;
using Ecommerce.BLL.Comparers;
using Ecommerce.BLL.Customers.Reviews.Customers;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.Applications;
using Ecommerce.BLL.Validation.Applications.ApplicationLabels;
using Ecommerce.BLL.Validation.Applications.ApplicationTags;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.ApplicationBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationSlider;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Applications
{
    public class ApplicationBLL : BaseBLL, IApplicationBLL
    {
        #region Fields

        IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ApplicationLabel> _applicationLabelRepository;
        private readonly IRepository<ApplicationTag> _applicationTagRepository;
        private readonly IRepository<ApplicationSlider> _applicationSliderRepository;
        private readonly IFileBLL _fileBLL;
        private readonly IBlobFileBLL _BlobfileBLL;
        private readonly IRepository<CountryCurrency> _countryCurrencyRepository;
        private readonly ICustomerReviewBLL _customerReviewBLL;
        private readonly IRepository<Version> _versionRepository;
        private readonly IRepository<VersionPrice> _versionPriceRepository;
        private readonly IRepository<VersionFeature> _versionFeatureRepository;
        private readonly IRepository<PriceLevel> _priceLevelRepository;
        private readonly IRepository<DevicesType> _devicesTypeRepository;
        private readonly IVersionBLL _versionBLL;
        private readonly FileStorageSetting _fileSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion

        #region Constructor

        public ApplicationBLL(IMapper mapper,
                              IUnitOfWork unitOfWork,
                              IRepository<Application> applicationRepository,
                              IRepository<Tag> tagRepository,
                              IRepository<ApplicationLabel> applicationLabelRepository,
                              IRepository<ApplicationTag> applicationTagRepository,
                              IRepository<ApplicationSlider> applicationSliderRepository,
                              IFileBLL fileBLL,
                              IBlobFileBLL blobFileBLL,
                              IRepository<Version> versionRepository,
                              IRepository<VersionPrice> versionPriceRepository,
                              ICustomerReviewBLL customerReviewBLL,
                              IRepository<VersionFeature> versionFeaturePriceRepository,
                              IRepository<PriceLevel> priceLevelRepository,
                              IVersionBLL versionBLL,
                              IRepository<CountryCurrency> countryCurrencyRepository,
                              IOptions<FileStorageSetting> fileSetting,
                              IWebHostEnvironment webHostEnvironment,

        IRepository<DevicesType> devicesTypeRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _tagRepository = tagRepository;
            _applicationLabelRepository = applicationLabelRepository;
            _applicationTagRepository = applicationTagRepository;
            _applicationTagRepository = applicationTagRepository;
            _applicationSliderRepository = applicationSliderRepository;
            _fileBLL = fileBLL;
            _versionRepository = versionRepository;
            _versionPriceRepository = versionPriceRepository;
            _customerReviewBLL = customerReviewBLL;
            _versionFeatureRepository = versionFeaturePriceRepository;
            _priceLevelRepository = priceLevelRepository;
            _versionBLL = versionBLL;
            _countryCurrencyRepository = countryCurrencyRepository;
            _devicesTypeRepository = devicesTypeRepository;
            _fileSetting = fileSetting.Value;
            _webHostEnvironment = webHostEnvironment;
            _BlobfileBLL = blobFileBLL;
        }
        #endregion

        #region Application
        public async Task<IResponse<GetApplicationOutputDto>> CreateAsync(CreateApplicationInputDto inputDto)
        {
            var output = new Response<GetApplicationOutputDto>();

            try
            {
                //Inputs Validation
                var validator = new CreateApplicationInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation.

                #region Comment
                ////check on (name and title)
                //if (IsApplicationNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                //    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Name));
                //if (IsApplicationNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                //    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Name));
                //if (IsApplicationTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Default))
                //    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Title));
                //if (IsApplicationTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Ar))
                //    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Title));

                //if (!output.IsSuccess)
                //    return output.CreateResponse();

                #endregion
                // upload image.
                var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                if (createdFileResult.Errors.Any())
                {
                    return output.AppendErrors(createdFileResult.Errors).CreateResponse();
                }

                var model = _mapper.Map<Application>(inputDto);

                if (model.DeviceTypeId == 0)
                    model.DeviceTypeId = 1;

                model.Image = createdFileResult.Data;
                _applicationRepository.Add(model);

                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetApplicationOutputDto>(model));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetApplicationOutputDto>> UpdateAsync(UpdateApplicationInputDto inputDto)
        {
            var output = new Response<GetApplicationOutputDto>();

            try
            {

                //TODO:Try to Refactor this
                FileStorage file = null;

                if (inputDto.File != null)
                {

                    // upload image
                    var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.ModifiedBy.GetValueOrDefault(0));

                    if (createdFileResult.Errors.Any())
                    {
                        return output.AppendErrors(createdFileResult.Errors).CreateResponse();

                    }
                    file = createdFileResult.Data;
                }
                ////////////////////////////////
                //Input Validation
                var validator = new UpdateApplicationInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                #region Comment
                //// 1- Check if Already Exists  (Name)
                //if (IsApplicationNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.Id))
                //    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Application.Name));
                //if (IsApplicationNameExist(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.Id))
                //    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Application.Name));
                //// 2- Check if Already Exists  (Title)
                //if (IsApplicationTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Default, inputDto.Id))
                //    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Application.Title));
                //if (IsApplicationTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), LangEnum.Ar, inputDto.Id))
                //    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Application.Title));

                //if (!output.IsSuccess)
                //    return output.CreateResponse();
                #endregion

                //Business
                var entity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Application));
                //3-Update Price
                #region UpdateVersionPriceBySubscriptionType
                UpdateVersionPriceBySubscriptionType(entity.SubscriptionTypeId, inputDto);
                #endregion

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //update Application
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Image = file;
                entity = _applicationRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetApplicationOutputDto>(entity));
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
                var entity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                // 1- Check if Entity has references
                var checkDto = EntityHasReferences(inputDto.Id, _applicationRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(ApplicationLabel)));
                if (checkDto.HasChildren == 0)
                {
                    //hard delete related entities
                    _applicationLabelRepository.Delete(x => x.ApplicationId == inputDto.Id);

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
        public async Task<IResponse<List<GetApplicationOutputDto>>> GetAllAsync()
        {
            var output = new Response<List<GetApplicationOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetApplicationOutputDto>>(_applicationRepository.GetAllList());

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<GetApplicationOutputDto>> GetByIdAsync(GetApplicationInputDto inputDto)
        {
            var output = new Response<GetApplicationOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetApplicationInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetApplicationOutputDto>(entity);

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
        public async Task<IResponse<PagedResultDto<GetApplicationOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetApplicationOutputDto>>();

            var result = GetPagedList<GetApplicationOutputDto, Application, int>(
                pagedDto: pagedDto,
                repository: _applicationRepository, orderExpression: x => x.Id,
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

        public async Task<IResponse<AppDetailsDto>> GetAppDetailsAsync(int appId, int countryId, int? currentUserId = null)
        {
            var response = new Response<AppDetailsDto>();

            try
            {
                var app = await _applicationRepository.GetByIdAsync(appId);

                if (app is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(Application));
                    return response;
                }

                var appDto = _mapper.Map<AppDetailsDto>(app);

                if (currentUserId != null)
                {
                    if (app.WishListApplications.Any(w => w.CustomerId == currentUserId))
                        appDto.IsInWishlist = true;
                }


                // related apps
                if (app.ApplicationTags.Any())
                {
                    var appTagsIds = app.ApplicationTags.Select(t => t.TagId).ToList();

                    // Todo-Sully: add fixed number "2" in appsettings.

                    var query = await _applicationTagRepository.Where(at => appTagsIds.Contains(at.TagId) && at.ApplicationId != app.Id)
                                                               .Select(at => at.Application)
                                                               .ToListAsync();

                    var relatedApps = query.Distinct(new ApplicationComparer()).Take(2);

                    appDto.RelatedApps = _mapper.Map<IEnumerable<RelatedAppDto>>(relatedApps);

                    if (currentUserId != null)
                    {
                        relatedApps.ToList().ForEach(a =>
                                           {
                                               if (a.WishListApplications.Any(w => w.CustomerId == currentUserId))
                                                   appDto.RelatedApps.FirstOrDefault(ra => ra.Id == a.Id).IsInWishlist = true;
                                           });
                    }

                }

                // rate.
                appDto.Rate = await _customerReviewBLL.GetRateAsync(appId, includeStarPercentage: true);

                var currencycode = _countryCurrencyRepository.Where(c => c.CountryId == countryId || c.DefaultForOther).Select(c => c.Currency.Code).FirstOrDefault();

                appDto.countryCurrency = currencycode;
                response.CreateResponse(appDto);
                return response;
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
                return response;
            }
        }
        public async Task<IResponse<GetAppDetailsPricingAndPlansOutputDto>> GetAppDetailsPricingAndPlans(GetAppDetailsPricingAndPlansInputDto inputDto)
        {
            var output = new Response<GetAppDetailsPricingAndPlansOutputDto>();
            try
            {
                //Business Validation
                var entity = _applicationRepository.Where(x => x.Versions.Any() && x.Id == inputDto.ApplicationId).FirstOrDefault();
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //get Application Features in All Versions
                var features = _versionFeatureRepository
                    .Where(x => x.Version != null && x.Version.Application != null &&
                    x.Version.ApplicationId == inputDto.ApplicationId).Select(x => x.Feature)
                    .Distinct()
                    .ToList();

                //get Application Prices in All Versions
                var packages = _priceLevelRepository
                    .Where(x => x.VersionPrices.Any(vp => vp.Version != null && vp.Version.Application != null && vp.Version.ApplicationId == inputDto.ApplicationId))
                    .ToList();
                //remove other Application Prices
                packages.ForEach(x =>
                {
                    var otherApplicationPackages = x.VersionPrices.Where(vp => vp.Version == null || vp.Version?.Application == null || vp.Version?.ApplicationId != inputDto.ApplicationId).ToList();
                    otherApplicationPackages.ForEach(o => x.VersionPrices.Remove(o));
                });

                //filter application
                var result = _applicationRepository
                 .Where(x => x.Versions.Any() && x.Id == inputDto.ApplicationId)
                 .Select(x => new GetAppDetailsPricingAndPlansOutputDto
                 {
                     Application = _mapper.Map<GetApplicationDropDownOutputDto>(entity),
                     //Versions = _mapper.Map<List<GetVersionsByApplicationAndPricingOutputDto>>(x.Versions),
                     Packages = _mapper.Map<List<GetApplicationPackagesOutputDto>>(packages),
                     Features = _mapper.Map<List<GetFeatureDropDownOutputDto>>(features)
                 })
                 .FirstOrDefault();

                #region default minimum package by minimum price else by licenses count
                var minimumVersionPrice = _versionBLL.GetApplicationMinimumVersionPriceHelper(inputDto.ApplicationId, inputDto.CountryId);
                var defaultPackage = 0;
                if (minimumVersionPrice is not null)
                {
                    defaultPackage = minimumVersionPrice.Values.FirstOrDefault().PriceLevelId;
                    if (defaultPackage > 0)
                        result.Packages.FirstOrDefault(x => x.Id == defaultPackage).IsDefault = true;
                    else
                        result.Packages.OrderBy(x => x.NumberOfLicenses).FirstOrDefault().IsDefault = true;

                }
                else
                    result.Packages.OrderBy(x => x.NumberOfLicenses).FirstOrDefault().IsDefault = true;
                #endregion

                output.CreateResponse(result);
                return output;
            }
            catch (Exception ex)
            {
                output.CreateResponse(ex);
                return output;
            }
        }

        public async Task<IResponse<List<PriceLevelResultDto>>> GetPackagesAsync(int versionId, int subscriptionTypeId, int countryId)
        {
            var response = new Response<List<PriceLevelResultDto>>();
            try
            {
                var versionPrices = _versionPriceRepository
                                                                      .Where(vp => vp.VersionId == versionId && vp.CountryCurrency.CountryId == countryId)
                                                                     .Select(vp => new PriceLevelResultDto
                                                                     {
                                                                         Id = vp.PriceLevelId,
                                                                         Price = subscriptionTypeId == (int)DiscriminatorsEnum.Forever ? vp.ForeverPrice :
                                                                                        subscriptionTypeId == (int)DiscriminatorsEnum.Monthly ? vp.MonthlyPrice :
                                                                                        subscriptionTypeId == (int)DiscriminatorsEnum.Yearly ? vp.YearlyPrice :
                                                                                        vp.MonthlyPrice,
                                                                         NumberOfLicences = vp.PriceLevel.NumberOfLicenses,
                                                                         Name = vp.PriceLevel.Name,
                                                                         Currency = vp.CountryCurrency.Currency.Code
                                                                     }).ToList();

                return response.CreateResponse(versionPrices);
            }
            catch (Exception e)
            {

                return response.CreateResponse(e);
            }
        }
        #endregion

        #region Application Labels

        public async Task<IResponse<GetApplicationLabelOutputDto>> CreateApplicationLabelAsync(CreateApplicationLabelInputDto inputDto)
        {
            var output = new Response<GetApplicationLabelOutputDto>();

            try
            {

                //Input Validation Validations
                var validator = new CreateApplicationLabelValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //check if Label Name  Length (Arabic and English) not exists before
                if (IsApplicationLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(ApplicationLabel.Name));
                if (IsApplicationLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(ApplicationLabel.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business Validations
                //1-check if Application Has Label Before 
                if (_applicationLabelRepository.Any(s => s.ApplicationId == inputDto.ApplicationId))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(ApplicationLabel));

                //Business
                var mappedInput = _mapper.Map<ApplicationLabel>(inputDto);

                _applicationLabelRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetApplicationLabelOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public async Task<IResponse<GetApplicationLabelOutputDto>> UpdateApplicationLabelAsync(UpdateApplicationLabelInputDto inputDto)
        {
            var output = new Response<GetApplicationLabelOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateApplicationLabelDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }



                //Business
                var entity = _applicationLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.ApplicationId == inputDto.ApplicationId);
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                //2-Check if Label Name  Length (Arabic and English) not exists before
                if (IsApplicationLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Default, entity.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(ApplicationLabel.Name));
                if (IsApplicationLabelNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Ar, entity.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(ApplicationLabel.Name));


                if (!output.IsSuccess)
                    return output.CreateResponse();

                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Tag
                entity = _applicationLabelRepository.Update(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetApplicationLabelOutputDto>(entity));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<bool>> DeleteApplicationLabelAsync(DeleteTrackedEntityInputDto inputDto)
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
                var entity = _applicationLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.ApplicationId == inputDto.Id);

                // 1-Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                //2-check if entity has related data
                var checkDto = EntityHasReferences(entity.Id, _applicationLabelRepository);
                if (checkDto.HasChildren == 0)
                {
                    entity.IsDeleted = true;
                    entity.ModifiedDate = inputDto.ModifiedDate;
                    entity.ModifiedBy = inputDto.ModifiedBy;
                    _applicationLabelRepository.Update(entity);

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


        public async Task<IResponse<GetApplicationLabelOutputDto>> GetApplicationLabelByApplicationIdAsync(GetApplicationLabelInputDto inputDto)
        {
            var output = new Response<GetApplicationLabelOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetApplicationLabelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _applicationLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.ApplicationId == inputDto.ApplicationId);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetApplicationLabelOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<List<GetApplicationLabelOutputDto>>> GetAllApplicationLabelListByApplicationIdAsync(GetApplicationLabelInputDto inputDto)
        {
            var output = new Response<List<GetApplicationLabelOutputDto>>();
            try
            {
                //Input Validation
                var validator = new GetApplicationLabelInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var result = _mapper.Map<List<GetApplicationLabelOutputDto>>(_applicationLabelRepository.Where(C => C.ApplicationId == inputDto.ApplicationId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<PagedResultDto<GetApplicationLabelOutputDto>>> GetApplicationLabelPagedListAsync(ApplicationFilteredPagedResult inputDto)
        {
            var output = new Response<PagedResultDto<GetApplicationLabelOutputDto>>();

            var result = GetPagedList<GetApplicationLabelOutputDto, ApplicationLabel, int>(
                pagedDto: inputDto,
                repository: _applicationLabelRepository,
                orderExpression: x => x.Id,
                searchExpression: x => x.ApplicationId == inputDto.ApplicationId && (string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm) &&
                         ((x.Name.Contains(inputDto.SearchTerm)
                      || x.Color.Contains(inputDto.SearchTerm)
                      || x.Application.Name.Contains(inputDto.SearchTerm))))),
                sortDirection: inputDto.SortingDirection,
                disableFilter: true);
            return output.CreateResponse(result);

        }

        #endregion

        #region Application Tags
        public async Task<IResponse<GetApplicationTagOutputDto>> CreateApplicationTagAsync(CreateApplicationTagInputDto inputDto)
        {
            var output = new Response<GetApplicationTagOutputDto>();

            try
            {

                //Input Validation Validations
                var validator = new CreateApplicationTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-check entity already exists
                if (IsApplicationTagExsistedBefore(inputDto))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(ApplicationTag));
                //Business
                var mappedInput = _mapper.Map<ApplicationTag>(inputDto);

                //check if this is the first record => set Featured = true;
                var firstRecord = !(_applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ApplicationId == inputDto.ApplicationId).Any());
                if (firstRecord)
                    mappedInput.IsFeatured = true;

                _applicationTagRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetApplicationTagOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        public async Task<IResponse<bool>> AssignFeaturedToApplicationTagAsync(int Id)
        {
            var output = new Response<bool>();

            try
            {
                //Business
                var entity = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == Id);
                // Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ApplicationTag));

                //update All other ApplicationTags to not featured 
                UpdateOldFeaturedApplicationTagToNotFeatured(entity.ApplicationId);

                entity.IsFeatured = true;
                //update ApplicationTag 
                entity = _applicationTagRepository.Update(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<List<GetTagOutputDto>>> GetAllActiveTagsNotAssignToApplicationIdAsync(int ApplicationId)
        {
            var output = new Response<List<GetTagOutputDto>>();
            try
            {
                //Business
                var ApplicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == ApplicationId);
                // 1- Check if Entity Exists
                if (ApplicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var ApplicationTags = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ApplicationId == ApplicationId).Select(f => f.TagId).ToList();
                var result = _mapper.Map<List<GetTagOutputDto>>(_tagRepository.Where(d => !ApplicationTags.Contains(d.Id)));
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetApplicationTagOutputDto>> UpdateApplicationTagAsync(UpdateApplicationTagInputDto inputDto)
        {
            var output = new Response<GetApplicationTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateApplicationTagDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Tag Assigned to this Application Before
                if (IsApplicationTagExsistedBefore(inputDto, inputDto.Id))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(ApplicationTag));

                //Business
                var entity = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ApplicationTag));
                //3-check if try to update IsFeatured to Not IsFeatured
                if (entity.IsFeatured && !inputDto.IsFeatured)
                {
                    //assign IsFeatured to the first entity
                    var FirstEntity = GetFirstApplicationTag(entity.ApplicationId);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                    //reject to update
                    else
                        return output.CreateResponse(MessageCodes.BusinessValidationError);
                }

                entity = _mapper.Map(inputDto, entity);
                //update Entity 
                entity = _applicationTagRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetApplicationTagOutputDto>(entity));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<bool>> DeleteApplicationTagAsync(DeleteEntityInputDto inputDto)
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
                var entity = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                //1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(ApplicationTag));

                //2-check if deleted Entity is Featured => Assign IsFeatured to The first Entity 
                if (entity.IsFeatured)
                {
                    var FirstEntity = GetFirstApplicationTag(entity.ApplicationId, entity.Id);
                    if (FirstEntity != null)
                        FirstEntity.IsFeatured = true;
                }

                //3-check if entity has related data
                var checkDto = EntityHasReferences(entity.Id, _applicationTagRepository);
                if (checkDto.HasChildren == 0)
                {
                    //Hard Delete Entity
                    _applicationTagRepository.Delete(entity);
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
        public async Task<IResponse<GetApplicationTagOutputDto>> GetApplicationTagByIdAsync(GetApplicationTagInputDto inputDto)
        {
            var output = new Response<GetApplicationTagOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetApplicationTagInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetApplicationTagOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<List<GetApplicationTagOutputDto>>> GetAllApplicationTagListByApplicationIdAsync(int ApplicationId)
        {
            var output = new Response<List<GetApplicationTagOutputDto>>();
            try
            {
                //Business
                var ApplicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == ApplicationId);
                // 1- Check if Entity Exists
                if (ApplicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var result = _mapper.Map<List<GetApplicationTagOutputDto>>(_applicationTagRepository.Where(C => C.ApplicationId == ApplicationId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<PagedResultDto<GetApplicationTagOutputDto>>> GetApplicationTagPagedListAsync(ApplicationFilteredPagedResult inputDto)
        {
            var output = new Response<PagedResultDto<GetApplicationTagOutputDto>>();
            //get active Tags only
            var activeTagIds = _tagRepository.GetAll().Select(x => x.Id).ToList();

            var result = GetPagedList<GetApplicationTagOutputDto, ApplicationTag, int>(
                pagedDto: inputDto,
                repository: _applicationTagRepository,
                orderExpression: x => x.Id,
                searchExpression: x => activeTagIds.Contains(x.TagId) && x.ApplicationId == inputDto.ApplicationId && (string.IsNullOrWhiteSpace(inputDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(inputDto.SearchTerm) && (x.Tag.Name.Contains(inputDto.SearchTerm)))),
                sortDirection: inputDto.SortingDirection,
                disableFilter: true,
                excluededColumns: null,
                 x => x.Tag, x => x.Application
            );
            return output.CreateResponse(result);

        }



        #endregion

        #region Application Sliders
        public async Task<IResponse<GetApplicationSliderOutputDto>> CreateApplicationSliderAsync(CreateApplicationSliderInputDto inputDto)
        {
            var output = new Response<GetApplicationSliderOutputDto>();

            try
            {

                //Input Validation
                var validator = new CreateApplicationSliderInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                // var fileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);
                var file = new SingleFilebaseDto
                {
                    FileDto = new FileDto
                    {
                        File = inputDto.File.File,
                        FilePath = _fileSetting.Files.Admin.Application.Path,
                        ContainerName = _fileSetting.Files.Admin.ContainerName,
                        FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory
                    }
                };

                var fileResult = await _BlobfileBLL.UploadFileAsync(file);


                if (!fileResult.IsSuccess)
                {
                    output.AppendErrors(fileResult.Errors);

                    return output.CreateResponse();
                }

                var mappedInput = _mapper.Map<ApplicationSlider>(inputDto);

                mappedInput.Media = fileResult.Data;

                _applicationSliderRepository.Add(mappedInput);

                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetApplicationSliderOutputDto>(mappedInput));

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<PagedResultDto<GetApplicationSliderOutputDto>>> GetAllApplicationSlidersPagedListAsync(ApplicationFilteredPagedResult pagedDto)
        {
            var result = new Response<PagedResultDto<GetApplicationSliderOutputDto>>();

            try
            {
                var output = GetPagedList<GetApplicationSliderOutputDto, ApplicationSlider, int>(pagedDto: pagedDto,
                                                                                        repository: _applicationSliderRepository,
                                                                                        orderExpression: s => s.Id,
                                                                                        searchExpression: s => s.ApplicationId == pagedDto.ApplicationId,
                                                                                        sortDirection: pagedDto.SortingDirection,
                                                                                        disableFilter: true, excluededColumns: null,
                                                                                        x => x.Application);
                return result.CreateResponse(output);
            }
            catch (Exception ex)
            {
                return result.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> DeleteApplicationSliderAsync(DeleteTrackedEntityInputDto inputDto)
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

                var sliderModel = _applicationSliderRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(s => s.Id == inputDto.Id);

                if (sliderModel is null)
                    return result.CreateResponse(MessageCodes.NotFound, nameof(ApplicationSlider));

                //check if entity has related data
                var checkDto = EntityHasReferences(sliderModel.Id, _applicationSliderRepository);
                if (checkDto.HasChildren == 0)
                {
                    sliderModel.IsDeleted = true;
                    sliderModel.ModifiedBy = inputDto.ModifiedBy;
                    sliderModel.ModifiedDate = inputDto.ModifiedDate;
                    _applicationSliderRepository.Update(sliderModel);
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

        #region DevicesType
        public async Task<IResponse<List<Ecommerce.DTO.Customers.DevicesType.GetDevicesTypeOutputDto>>> GetAllDevicesTypeAsync()
        {
            var output = new Response<List<Ecommerce.DTO.Customers.DevicesType.GetDevicesTypeOutputDto>>();
            try
            {
                var devicesType = _devicesTypeRepository.GetAll().ToList();
                var mappedResult = _mapper.Map<List<Ecommerce.DTO.Customers.DevicesType.GetDevicesTypeOutputDto>>(devicesType);

                return output.CreateResponse(mappedResult);

            }
            catch (Exception e)
            {
                return output.CreateResponse(e);

            }
        }
        #endregion

        public async Task<DownloadPriceDto> GetMimumPriceWithCurrencyId(Application application, int currencyId)
        {
            try
            {
                var versions = _versionRepository.Where(v => v.ApplicationId == application.Id).AsEnumerable();
                var x = versions?.SelectMany(v => v.VersionPrices)?.Where(vp => vp.CountryCurrencyId == currencyId);
                var monthlyPrice = versions?.SelectMany(v => v.VersionPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.MonthlyNetPrice).DefaultIfEmpty(default).Min();
                var yearlyPrice = versions?.SelectMany(v => v.VersionPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.YearlyNetPrice).DefaultIfEmpty(default).Min();
                var foreverPrice = versions?.SelectMany(v => v.VersionPrices)?.Where(vp => vp.CountryCurrencyId == currencyId)?.Select(x => x.ForeverNetPrice).DefaultIfEmpty(default).Min();
                return new DownloadPriceDto
                {
                    MonthlyPrice = monthlyPrice ?? default,
                    YearlyPrice = yearlyPrice ?? default,
                    ForeverPrice = foreverPrice ?? default
                };
            }
            catch (Exception e)
            {

                throw;
            }

        }

        #region Helpers

        #region Application

        private bool IsApplicationNameExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? ApplicationId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
                     ? _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim())
                                    .WhereIf(a => a.Id != ApplicationId, ApplicationId.HasValue).Any()
                     : _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                                    .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim())
                                    .WhereIf(a => a.Id != ApplicationId, ApplicationId.HasValue).Any();
        }
        private bool IsApplicationTitleExist(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? ApplicationId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
            ? _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.default") == name.Default.Trim())
                           .WhereIf(a => a.Id != ApplicationId, ApplicationId.HasValue).Any()
            : _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                           .Where(a => Json.Value(a.Title, "$.ar") == name.Ar.Trim())
                           .WhereIf(a => a.Id != ApplicationId, ApplicationId.HasValue).Any();
        }

        public void UpdateVersionPriceBySubscriptionType(int subscriptionId, UpdateApplicationInputDto inputDto)
        {
            var prices = _versionPriceRepository.Where(x => x.Version.Application.Id == inputDto.Id);
            if (subscriptionId == (int)SubscriptionTypeEnum.Forever && inputDto.SubscriptionTypeId == (int)SubscriptionTypeEnum.Others)
            {
                foreach (var price in prices)
                {
                    price.ForeverNetPrice = 0;
                    price.ForeverPrice = 0;
                    price.ForeverPrecentageDiscount = 0;
                    _versionPriceRepository.Update(price);
                }
            }
            if (subscriptionId == (int)SubscriptionTypeEnum.Others && inputDto.SubscriptionTypeId == (int)SubscriptionTypeEnum.Forever)
            {
                foreach (var price in prices)
                {
                    price.MonthlyNetPrice = 0;
                    price.MonthlyPrecentageDiscount = 0;
                    price.MonthlyPrice = 0;
                    price.YearlyPrice = 0;
                    price.YearlyNetPrice = 0;
                    price.YearlyPrecentageDiscount = 0;
                    _versionPriceRepository.Update(price);
                }
            }
        }
        #endregion

        #region Application Label
        private bool IsApplicationLabelNameExist(JsonLanguageModel name, int applicationId, LangEnum lang = LangEnum.Default, int? excludedId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
              ? _applicationLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any()
              : _applicationLabelRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any();
        }
        #endregion

        #region Application Tag
        public bool IsApplicationTagExsistedBefore(ApplicationTagDto dto, int? Id = null)
        {
            return _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != Id, Id.HasValue).Any(s => s.ApplicationId == dto.ApplicationId && s.TagId == dto.TagId);
        }
        private List<ApplicationTag> UpdateOldFeaturedApplicationTagToNotFeatured(int ApplicationId)
        {
            var oldFeatured = _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ApplicationId == ApplicationId && x.IsFeatured).ToList();
            oldFeatured.ForEach(c =>
            {
                c.IsFeatured = false;
                _applicationTagRepository.Update(c);

            });
            return oldFeatured;
        }

        private ApplicationTag GetFirstApplicationTag(int ApplicationId, int? ApplicationTagId = null)
        {
            return _applicationTagRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != ApplicationTagId, ApplicationTagId.HasValue).Where(f => f.ApplicationId == ApplicationId).OrderBy(f => f.Id).FirstOrDefault();
        }


        #endregion
        #endregion
    }
}
