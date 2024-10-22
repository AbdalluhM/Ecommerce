using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Countries;
using Ecommerce.BLL.Employees;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation;
using Ecommerce.BLL.Validation.Applications.Versions;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Applications.Versions
{
    public class VersionBLL : BaseBLL, IVersionBLL
    {
        #region Fields

        IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Version> _versionRepository;

        private readonly IRepository<VersionRelease> _versionReleaseRepository;

        private readonly IRepository<Application> _applicationRepository;

        IRepository<VersionPrice> _versionPriceRepository;
        IRepository<CountryCurrency> _countryCurrencyRepository;
        IRepository<PriceLevel> _priceLevelRepository;
        IRepository<ViewMissingVersionPrice> _missingVersionPriceRepository;

        private readonly IFileBLL _fileBLL;
        private readonly ICountryBLL _countryBLL;
        private readonly IEmployeeBLL _employeeBLL;
        #endregion

        #region Constructor

        public VersionBLL(IMapper mapper,
                          IUnitOfWork unitOfWork,
                          IRepository<Version> versionRepository,
                          IRepository<VersionRelease> versionReleaseRepository,
                          IRepository<Application> applicationRepository,
                          IRepository<VersionPrice> versionPriceRepository,
                          IRepository<CountryCurrency> countryCurrencyRepository,
                          IRepository<PriceLevel> priceLevelRepository,
                          IRepository<ViewMissingVersionPrice> missingVersionPriceRepository,
                          IFileBLL fileBLL,
                          ICountryBLL countryBLL,
                          IEmployeeBLL employeeBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _versionRepository = versionRepository;
            _versionReleaseRepository = versionReleaseRepository;
            _applicationRepository = applicationRepository;
            _versionPriceRepository = versionPriceRepository;
            _countryCurrencyRepository = countryCurrencyRepository;
            _priceLevelRepository = priceLevelRepository;
            _missingVersionPriceRepository = missingVersionPriceRepository;
            _fileBLL = fileBLL;
            _countryBLL = countryBLL;
            _employeeBLL = employeeBLL;
        }
        #endregion

        #region Version
        public async Task<IResponse<GetVersionOutputDto>> CreateAsync(CreateVersionInputDto inputDto)
        {
            var output = new Response<GetVersionOutputDto>();

            try
            {
                //Inputs Validation
                var validator = new CreateVersionInputDtoValidator().Validate(inputDto);

                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation.
                // 1- check on MaxActiveVersisons Count to be < 3 before creating a new active version
                if (inputDto.IsActive)
                {
                    int activeVersionsCount = GetActiveVersionsCount(inputDto.ApplicationId, null);
                    if (activeVersionsCount >= General.MaxVersionsCount)
                        return output.CreateResponse(MessageCodes.MaxActiveVersionsCountLimitExceeded);
                }

                //2- check if entity is highlighted and not active
                if (!inputDto.IsActive && inputDto.IsHighlightedVersion)
                    return output.CreateResponse(MessageCodes.InActiveEntity, nameof(Version));
                //3-check on (name and title)

                if (IsVersionNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Version.Name));
                if (IsVersionNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Version.Name));
                if (IsVersionTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), inputDto.ApplicationId, LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Version.Title));
                if (IsVersionTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), inputDto.ApplicationId, LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Version.Title));
                if (!string.IsNullOrEmpty(inputDto.ProductCrmId) && _versionRepository.Any(v => v.ProductCrmId == inputDto.ProductCrmId))
                    output.AppendError(MessageCodes.AlreadyExists, nameof(Version.ProductCrmId));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                var createdVersions = await _versionRepository.Where(v => v.ApplicationId == inputDto.ApplicationId).ToListAsync();

                if (!createdVersions.Any())
                {
                    inputDto.IsHighlightedVersion = true;
                }
                else
                {
                    if (inputDto.IsHighlightedVersion)
                    {
                        createdVersions.ForEach(cv => cv.IsHighlightedVersion = false);
                    }
                }

                // Old code by Saeed.
                //// Check any version is higlighted
                //var versions = _versionRepository.Where(x => x.IsHighlightedVersion).ToList();
                //if (versions == null)
                //{
                //    inputDto.IsHighlightedVersion = true;
                //}

                //Remove VersionRelease Url uniquness
                ////4-Check DownloadUrl is exisit
                //var checkedDownloadUrl = _versionReleaseRepository.Any(x => x.DownloadUrl == inputDto.DownloadUrl);
                //if (checkedDownloadUrl)
                //    return output.CreateResponse(MessageCodes.VersionReleaseExisit);
                // upload image.
                var createdFileResult = _fileBLL.UploadFile(inputDto.File, inputDto.CreatedBy);

                if (createdFileResult.Errors.Any())
                {
                    return output.AppendErrors(createdFileResult.Errors).CreateResponse();
                }

                var mappedInput = _mapper.Map<Version>(inputDto);
                mappedInput.Image = createdFileResult.Data;
                mappedInput.VersionReleases.Add(new VersionRelease()
                {
                    VersionId = mappedInput.Id,
                    CreateDate = mappedInput.CreateDate,
                    CreatedBy = mappedInput.CreatedBy,
                    DownloadUrl = inputDto.DownloadUrl,
                    ReleaseNumber = inputDto.ReleaseNumber,
                    IsCurrent = true
                });


                //update  Old Higlighted Version To Not HighLighted if current new one is highlighted
                if (inputDto.IsHighlightedVersion)
                    UpdateOldHiglightedVersionToNotHighLighted(inputDto.ApplicationId);

                _versionRepository.Add(mappedInput);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetVersionOutputDto>(mappedInput));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetVersionOutputDto>> UpdateAsync(UpdateVersionInputDto inputDto)
        {
            var output = new Response<GetVersionOutputDto>();

            try
            {

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
                var validator = new UpdateVersionInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- check on MaxActiveVersisons Count to be < 3 before creating a new active version
                if (inputDto.IsActive)
                {
                    int activeVersionsCount = GetActiveVersionsCount(inputDto.ApplicationId, inputDto.Id);
                    if (activeVersionsCount >= General.MaxVersionsCount)
                        return output.CreateResponse(MessageCodes.MaxActiveVersionsCountLimitExceeded);
                }
                // 2- Check if Already Exists  (Name)
                if (IsVersionNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Version));
                else if (IsVersionNameExist(GetJsonLanguageModelOrNull(inputDto.Name), inputDto.ApplicationId, LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Version));
                // 3- Check if Already Exists  (Title)
                if (IsVersionTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), inputDto.ApplicationId, LangEnum.Default, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Version));
                else if (IsVersionTitleExist(GetJsonLanguageModelOrNull(inputDto.Title), inputDto.ApplicationId, LangEnum.Ar, inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Version));
                if (!string.IsNullOrEmpty(inputDto.ProductCrmId) && _versionRepository.Any(v => v.ProductCrmId == inputDto.ProductCrmId && v.Id != inputDto.Id))
                    output.AppendError(MessageCodes.AlreadyExists, nameof(Version.ProductCrmId));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                //Business
                var entity = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Version));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                // Check any version is higlighted
                if (entity.IsHighlightedVersion && !inputDto.IsHighlightedVersion)
                    return output.CreateResponse(MessageCodes.HiglightedVersion, nameof(Version));

                //update Version
                entity = _mapper.Map(inputDto, entity);
                if (file != null)
                    entity.Image = file;

                //3-check if try to update IsHighlightedVersion to Not IsHighlightedVersion
                if (inputDto.IsHighlightedVersion)
                {
                    //update Update Old Higlighted Version To Not HighLighted if current new one is highlighted
                    UpdateOldHiglightedVersionToNotHighLighted(inputDto.ApplicationId, entity.Id);
                }
                //4- check if entity is highlighted and not active
                if (!inputDto.IsActive && inputDto.IsHighlightedVersion)
                    return output.CreateResponse(MessageCodes.InActiveEntity, nameof(Version));

                //5-check if versionrelease exists
                //Remove VersionRelease Url uniquness

                if (entity.VersionReleases.Any() && !string.IsNullOrWhiteSpace(inputDto.DownloadUrl) && !string.IsNullOrWhiteSpace(inputDto.ReleaseNumber))
                {
                    var versionReleases = entity.VersionReleases.Any(x => x.ReleaseNumber == inputDto.ReleaseNumber/* || x.DownloadUrl == inputDto.DownloadUrl*/);
                    if (!versionReleases)
                    {
                        //Add new vesion release and delete old
                        var currentreleaseVersions = entity.VersionReleases.Where(x => x.VersionId == inputDto.Id).ToList();

                        foreach (var item in currentreleaseVersions)
                        {
                            item.IsCurrent = false;
                        }
                        entity.VersionReleases.Add(new VersionRelease()
                        {
                            VersionId = entity.Id,
                            CreateDate = DateTime.UtcNow,
                            CreatedBy = entity.CreatedBy,
                            DownloadUrl = inputDto.DownloadUrl,
                            ReleaseNumber = inputDto.ReleaseNumber,
                            IsCurrent = true
                        });
                    }
                }
                entity = _versionRepository.Update(entity);
                _unitOfWork.Commit();

                return output.CreateResponse(_mapper.Map<GetVersionOutputDto>(entity));
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
                var entity = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Version));
                // 2- Check if Entity is HiglightedVersion
                if (entity.IsHighlightedVersion)
                    return output.CreateResponse(MessageCodes.HiglightedVersion, nameof(Version));

                // 3- Check if Entity has references
                var checkDto = EntityHasReferences(inputDto.Id, _versionRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(VersionRelease)));
                if (checkDto.HasChildren == 0)
                {
                    //hard delete related entity
                    _versionReleaseRepository.Delete(x => x.VersionId == entity.Id);

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
        public async Task<IResponse<List<GetVersionOutputDto>>> GetAllAsync(int? applicationId = null)
        {
            var output = new Response<List<GetVersionOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetVersionOutputDto>>(_versionRepository.WhereIf(x => x.ApplicationId == applicationId, applicationId.HasValue && applicationId.Value > 0).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<IEnumerable<BaseLookupDto>>> GetAppVersionsLookupAsync(int appId)
        {
            var response = new Response<IEnumerable<BaseLookupDto>>();

            var versions = await _versionRepository.Where(v => v.ApplicationId == appId)
                .OrderBy(v => !v.IsHighlightedVersion)
                .ToListAsync();

            if (versions is null || !versions.Any())
                return response.CreateResponse(data: null);

            var versionsDto = _mapper.Map<IEnumerable<BaseLookupDto>>(versions);

            return response.CreateResponse(versionsDto);
        }

        public async Task<IResponse<GetVersionOutputDto>> GetByIdAsync(GetVersionInputDto inputDto)
        {
            var output = new Response<GetVersionOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetVersionInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetVersionOutputDto>(entity);

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
        public async Task<IResponse<PagedResultDto<GetVersionOutputDto>>> GetPagedListAsync(ApplicationFilteredPagedResult pagedDto)
        {
            var output = new Response<PagedResultDto<GetVersionOutputDto>>();

            var result = GetPagedList<GetVersionOutputDto, Version, int>(
                pagedDto: pagedDto,
                repository: _versionRepository, orderExpression: x => x.Id,
                searchExpression: x => x.ApplicationId == pagedDto.ApplicationId && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.Name.Contains(pagedDto.SearchTerm)
                      || x.Title.Contains(pagedDto.SearchTerm)
                      || x.MainPageUrl.Contains(pagedDto.SearchTerm)
                      || x.ShortDescription.Contains(pagedDto.SearchTerm)))),
               sortDirection: pagedDto.SortingDirection,
                      disableFilter: true,
                      excluededColumns: null,
              x => x.Application);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<VersionDetailsDto>> GetVersionDetailsAsync(int appId, int countryId, int? versionId = null)
        {
            var response = new Response<VersionDetailsDto>();

            try
            {
                Version version;

                if (versionId.HasValue)
                {
                    version = await _versionRepository.GetByIdAsync(versionId.Value);

                    if (version is null)
                    {
                        response.CreateResponse(MessageCodes.NotFound, nameof(Version));
                        return response;
                    }
                }
                else
                {
                    var minimumVersionPrice = GetApplicationMinimumVersionPriceHelper(appId, countryId);

                    if (minimumVersionPrice is null)
                    {
                        response.CreateResponse(MessageCodes.NotFound, nameof(Version));
                        return response;
                    }

                    version = minimumVersionPrice.Values.FirstOrDefault().Version;
                }
                response.CreateResponse(_mapper.Map<VersionDetailsDto>(version));
                return response;
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
                return response;
            }
        }


        public async Task<IResponse<List<GetVersionsByApplicationAndPricingOutputDto>>> GetAvailableVersionsByApplicationAndPricing(GetAvailableVersionsByApplicationAndPricingInputDto inputDto)
        {
            var output = new Response<List<GetVersionsByApplicationAndPricingOutputDto>>();

            try
            {
                //Business Validation
                var application = _applicationRepository.Where(x => x.Id == inputDto.ApplicationId).FirstOrDefault();
                if (application == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                if (inputDto.PriceLevelId.HasValue && inputDto.PriceLevelId.Value > 0)
                {
                    var priceLevel = _priceLevelRepository.Where(x => x.Id == inputDto.PriceLevelId).FirstOrDefault();
                    if (priceLevel == null)
                        return output.CreateResponse(MessageCodes.FailedToFetchData);
                }

                //get all versions related to application
                var result = _versionRepository
                 //.WhereIf(
                 //   x => x.VersionPrices.Any(vp => vp.PriceLevelId == inputDto.PriceLevelId),
                 //   inputDto.PriceLevelId.HasValue && inputDto.PriceLevelId.Value > 0
                 //)
                 .Where(x =>
                 x.Application != null
                 && x.ApplicationId == inputDto.ApplicationId
                 )
                 .ToList();

                var mappedResult = _mapper.Map<List<GetVersionDataOutputDto>>(result);
                //remove verisonprices that not exist with pricelevelid

                mappedResult.ForEach(x =>
                       {
                           if (inputDto.PriceLevelId.HasValue && inputDto.PriceLevelId.Value > 0)
                               x.VersionPrices.RemoveAll(vp => inputDto.PriceLevelId.Value != vp.PriceLevelId);
                           x.MinVersionPrice = new VersionPriceDto
                           {
                               VersionId = x.Id
                               ,
                               PriceLevelId = inputDto.PriceLevelId.GetValueOrDefault(0)
                           };
                       });


                output.CreateResponse(_mapper.Map<List<GetVersionsByApplicationAndPricingOutputDto>>(mappedResult));
                return output;
            }
            catch (Exception ex)
            {
                output.CreateResponse(ex);
                return output;
            }
        }

        public async Task<IResponse<List<GetVersionsByApplicationAndPricingOutputDto>>> GetAvailableVersionsByApplication(int appId, int countryId)
        {
            var output = new Response<List<GetVersionsByApplicationAndPricingOutputDto>>();

            try
            {
                //Business Validation
                var application = _applicationRepository.Where(x => x.Id == appId).FirstOrDefault();
                if (application == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //get all versions related to application
                var result = _versionRepository
                 //.WhereIf(
                 //   x => x.VersionPrices.Any(vp => vp.PriceLevelId == inputDto.PriceLevelId),
                 //   inputDto.PriceLevelId.HasValue && inputDto.PriceLevelId.Value > 0
                 //)
                 .Where(x =>
                 x.Application != null
                 && x.ApplicationId == appId
                 )
                 .ToList();

                var versionPrices = _versionPriceRepository.Where(x => result.Select(x => x.Id).Contains(x.VersionId)).ToList();
                var mappedResult = _mapper.Map<List<GetVersionDataOutputDto>>(result);
                mappedResult.ForEach(x =>
                {
                    x.NotAvailable = !versionPrices.Where(vp => vp.VersionId == x.Id && vp.CountryCurrency?.CountryId == countryId).Any();

                    x.MinVersionPrice = new VersionPriceDto
                    {
                        VersionId = x.Id
                        ,
                        PriceLevelId = 0
                    };
                });

                var getVersionsByApplicationAndPricingOutputDtos = _mapper.Map<List<GetVersionsByApplicationAndPricingOutputDto>>(mappedResult);
                output.CreateResponse(getVersionsByApplicationAndPricingOutputDtos);
                return output;
            }
            catch (Exception ex)
            {
                output.CreateResponse(ex);
                return output;
            }
        }



        public VersionForBuyNowOutputDto GetVersionDataForBuyNow(PaymentDetailsInputDto inputDto)
        {
            var output = new VersionForBuyNowOutputDto();

            try
            {
                bool isfirstInvoice = !((inputDto.VersionSubscriptionId > 0 && !inputDto.IsAddOn) || (inputDto.AddOnSubscriptionId > 0 && inputDto.IsAddOn));

                //Business Validation
                var version = isfirstInvoice
                    ? _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Application != null && x.Id == inputDto.VersionId).FirstOrDefault()
                    : _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Application != null && x.Id == inputDto.VersionId).FirstOrDefault();


                var applicaion = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(a => a.Id == version.ApplicationId);

                if (version == null)
                    return null;
                //if (!inputDto.IsAddOn) 
                //{ 
                if (inputDto.PriceLevelId > 0)
                {
                    var priceLevel = isfirstInvoice
                        ? _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == inputDto.PriceLevelId).FirstOrDefault()
                        : _priceLevelRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == inputDto.PriceLevelId).FirstOrDefault();

                    if (priceLevel == null)
                        return null;
                }
                //}
                var mappedResult = _mapper.Map<GetVersionDataOutputDto>(version);
                //remove verisonprices that not exist with pricelevelid
                if (inputDto.PriceLevelId > 0)
                    mappedResult.VersionPrices.RemoveAll(vp => inputDto.PriceLevelId != vp.PriceLevelId);
                mappedResult.MinVersionPrice = new VersionPriceDto { VersionId = mappedResult.Id, PriceLevelId = inputDto.PriceLevelId };

                //mappedResult.Packages = _mapper.Map<List<GetApplicationPackagesOutputDto>>(packages);
                var result = _mapper.Map<VersionForBuyNowOutputDto>(mappedResult);
                result.ApplicationTitle = applicaion.Title;
                result.ApplicationId = applicaion.Id;
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Version Releases
        public VersionRelease GetCurrentVersionReleaseOrNull(int versionId, bool isFirstSubscription = true)
        {
            var version = isFirstSubscription
                        ? _versionRepository.GetById(versionId)
                       : _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == versionId);

            return _versionReleaseRepository.Where(x => x.VersionId == version.Id && x.IsCurrent).FirstOrDefault();
            //if (version != null && version.VersionReleases.Any(x => x.IsCurrent))
            //    return version.VersionReleases.FirstOrDefault(x => x.IsCurrent);
            //else
            //    return null;
        }
        public VersionRelease GetVersionReleaseById(int versionReleaseId)
        {
            var versionRelease = _versionReleaseRepository.GetById(versionReleaseId);
            return versionRelease;
        }

        #endregion

        #region Version Prices

        public async Task<IResponse<GetVersionPriceOutputDto>> CreateVersionPriceAsync(CreateVersionPriceInputDto inputDto)
        {
            var output = new Response<GetVersionPriceOutputDto>();
            try
            {
                //Input Validation Validations
                var validator = new CreateVersionPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                //Business Validations
                //1-check entity already exists
                if (IsVersionPriceAlreadExsist(inputDto))
                {
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(VersionPrice));
                }
                //Business
                var entity = _mapper.Map<VersionPrice>(inputDto);
                await _versionPriceRepository.AddAsync(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionPriceOutputDto>(entity);
                //entity = _versionPriceRepository.FindByInclude(x=> x.Id == entity
                //.Id,x => x.PriceLevel).FirstOrDefault();
                return output.CreateResponse(result);

                //return output.WithData(result).CreateResponse();

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);


            }
        }
        public async Task<IResponse<GetVersionPriceOutputDto>> UpdateVersionPriceAsync(UpdateVersionPriceInputDto inputDto)
        {
            var output = new Response<GetVersionPriceOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateVersionPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validations
                // 1- Check if Already Exists VersionPrice 
                if (IsVersionPriceAlreadExsist(inputDto, inputDto.Id))
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(VersionPrice));


                //Business
                var entity = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);

                // 2- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionPrice));


                entity = _mapper.Map(inputDto, entity);
                //update VersionPrice
                entity = _versionPriceRepository.Update(entity);
                _unitOfWork.Commit();

                var result = _mapper.Map<GetVersionPriceOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }

        }
        public async Task<IResponse<bool>> DeleteVersionPriceAsync(DeleteTrackedEntityInputDto inputDto)
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
                var entity = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionPrice));

                var checkDto = EntityHasReferences(inputDto.Id, _versionPriceRepository);
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
        public async Task<IResponse<List<GetVersionPriceOutputDto>>> GetAllExistingVersionPricesAsync(GetApplicationVersionPricesInputDto inputDto)
        {
            var output = new Response<List<GetVersionPriceOutputDto>>();
            try
            {
                var validator = new GetVersionPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var result = _mapper.Map<List<GetVersionPriceOutputDto>>(_versionPriceRepository.Where(x => x.Version != null && x.Version.ApplicationId == inputDto.ApplicationId).ToList());
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<List<GetVersionPriceOutputDto>>> GetAllMissingVersionPricesAsync(GetApplicationVersionPricesInputDto inputDto)
        {
            var output = new Response<List<GetVersionPriceOutputDto>>();
            try
            {
                var validator = new GetVersionPricesInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }
                var result = _missingVersionPriceRepository.Where(x => x.ApplicationId == inputDto.ApplicationId).ToList();
                var missingVersionPrices = _mapper.Map<List<GetVersionPriceOutputDto>>(result);
                return output.CreateResponse(missingVersionPrices);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<GetVersionPriceOutputDto>> GetVersionPriceByIdAsync(GetVersionPriceInputDto inputDto)
        {
            var output = new Response<GetVersionPriceOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetVersionPriceInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entity = _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.Id == inputDto.Id).FirstOrDefault();
                if (entity == null)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                //Business
                var result = _mapper.Map<GetVersionPriceOutputDto>(entity);
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
        public async Task<IResponse<PagedResultDto<GetVersionPriceOutputDto>>> GetAllExistingVersionPricePagedListAsync(ApplicationFilteredPagedResult pagedDto, int currentEmployeeId)
        {
            var output = new Response<PagedResultDto<GetVersionPriceOutputDto>>();

            var countryCurrenciesIds = _employeeBLL.GetEmployeeCountryCurrencies(currentEmployeeId);
            //var activeVersionIds = _versionRepository.Where(x => x.ApplicationId == pagedDto.ApplicationId).Select(x => x.Id).ToList();
            var result = GetPagedList<GetVersionPriceOutputDto, VersionPrice, int>(
                pagedDto: pagedDto,
                repository: _versionPriceRepository,
                orderExpression: x => x.Id,
                searchExpression: x =>/* activeVersionIds.Contains(x.VersionId) &&*/ x.Version.ApplicationId == pagedDto.ApplicationId && countryCurrenciesIds.Contains(x.CountryCurrencyId) && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         ((x.CountryCurrency.Country.Name.Contains(pagedDto.SearchTerm)
                      || x.CountryCurrency.Currency.Symbole.Contains(pagedDto.SearchTerm)
                      || x.PriceLevel.Name.Contains(pagedDto.SearchTerm))))),
               sortDirection: nameof(SortingDirection.ASC),
               disableFilter: true,
               excluededColumns: null,
              x => x.Version.Application, x => x.CountryCurrency, x => x.PriceLevel);
            return output.CreateResponse(result);
        }

        public async Task<IResponse<PagedResultDto<GetVersionPriceOutputDto>>> GetAllMissingVersionPricesPagedListAsync(ApplicationFilteredPagedResult pagedDto, int currentEmployeeId)
        {
            var output = new Response<PagedResultDto<GetVersionPriceOutputDto>>();

            var countryCurrenciesIds = _employeeBLL.GetEmployeeCountryCurrencies(currentEmployeeId);

            try
            {
                var result = GetPagedList<GetVersionPriceOutputDto, ViewMissingVersionPrice, int>(pagedDto: pagedDto, repository: _missingVersionPriceRepository, orderExpression: x => x.VersionId,
                      searchExpression: x => x.ApplicationId == pagedDto.ApplicationId && countryCurrenciesIds.Contains(x.CountryCurrencyId) && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.CountryName.Contains(pagedDto.SearchTerm)
                      || x.CurrencyShortCode.Contains(pagedDto.SearchTerm)
                      || x.PriceLevelName.Contains(pagedDto.SearchTerm)))),
                  sortDirection: pagedDto.SortingDirection,
                  disableFilter: true);

                return output.CreateResponse(result);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        #endregion

        #region Helpers

        #region Totals
        public int GetMissingPriceCount(int applicationId, IEnumerable<int> countryCurrencyIds)
        {
            return _missingVersionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Count(x => x.ApplicationId == applicationId && countryCurrencyIds.Contains(x.CountryCurrencyId));
        }

        public int GetApplicationVersionsCount(int applicationId)
        {
            return _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ApplicationId == applicationId).Select(x => x.ApplicationId).Count();
        }

        public VersionPriceDetailsDto GetApplicationMinimumVersionPrice(int applicationId, int? countryCurrencyId = null, int? priceLevelId = null)
        {
            try
            {

                var minimumPriceDic = GetApplicationMinimumVersionPriceHelper(applicationId, countryCurrencyId, priceLevelId);

                if (minimumPriceDic is null)
                    return null;

                var subscriptionType = minimumPriceDic.Keys.FirstOrDefault();





                if (subscriptionType == SubscriptionTypeEnum.Forever)
                    return _mapper.Map<ForeverPriceDetailsDto>(minimumPriceDic[subscriptionType]);
                if (subscriptionType == SubscriptionTypeEnum.Others)
                    return minimumPriceDic[subscriptionType].MonthlyNetPrice < minimumPriceDic[subscriptionType].YearlyNetPrice ? _mapper.Map<MonthlyPriceDetailsDto>(minimumPriceDic[subscriptionType]) : _mapper.Map<YearlyPriceDetailsDto>(minimumPriceDic[subscriptionType]);

                var minNetPrice = Math.Min(Math.Min(minimumPriceDic[subscriptionType].MonthlyNetPrice, minimumPriceDic[subscriptionType].ForeverNetPrice), minimumPriceDic[subscriptionType].YearlyNetPrice);
                // Determine which price and discount to use based on the minimum net price

                if (minNetPrice == minimumPriceDic[subscriptionType].MonthlyNetPrice)
                {
                    return _mapper.Map<MonthlyPriceDetailsDto>(minimumPriceDic[subscriptionType]);
                }

                if (minNetPrice == minimumPriceDic[subscriptionType].YearlyNetPrice)
                {
                    return _mapper.Map<YearlyPriceDetailsDto>(minimumPriceDic[subscriptionType]);

                }

                // minNetPrice == src.ForeverNetPrice
                return _mapper.Map<ForeverPriceDetailsDto>(minimumPriceDic[subscriptionType]);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public VersionPriceAllDetailsDto GetApplicationMinimumVersionPrices(int applicationId, int? countryCurrencyId = null, int? priceLevelId = null)
        {
            try
            {

                var minimumPriceDic = GetApplicationMinimumVersionPriceHelper(applicationId, countryCurrencyId, priceLevelId);

                if (minimumPriceDic is null)
                    return null;

                var subscriptionType = minimumPriceDic.Keys.FirstOrDefault();


                return new VersionPriceAllDetailsDto
                {
                    Forever = minimumPriceDic != null ? _mapper.Map<ForeverPriceDetailsDto>(minimumPriceDic.FirstOrDefault().Value) : null,
                    Monthly = minimumPriceDic != null ? _mapper.Map<MonthlyPriceDetailsDto>(minimumPriceDic.FirstOrDefault().Value) : null,
                    Yearly = minimumPriceDic != null ? _mapper.Map<YearlyPriceDetailsDto>(minimumPriceDic.FirstOrDefault().Value) : null,
                };



            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Todo-Sully: Get country currency from token.
        public VersionPriceDetailsDto GetMinimumVersionPrice(int versionId, int? countryId = null, int? priceLevelId = null)
        {
            try
            {

                var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;
                var versions = _versionPriceRepository
                           .WhereIf(vp => vp.CountryCurrency != null && vp.CountryCurrency.Id == countryCurrencyId, countryCurrencyId.HasValue && countryCurrencyId.Value > 0)
                           .WhereIf(vp => vp.PriceLevelId == priceLevelId, priceLevelId.HasValue && priceLevelId.Value > 0)
                           .Where(vp => vp.VersionId == versionId);

                var app = versions.Select(v => v.Version.Application).FirstOrDefault();

                if (app is null)
                    return null;

                var minVersionPrice = new VersionPrice();


                switch ((SubscriptionTypeEnum)app.SubscriptionTypeId)
                {
                    case SubscriptionTypeEnum.Forever:
                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                            ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                            : versions?
                                .OrderBy(vp => vp.ForeverNetPrice)
                                .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                .FirstOrDefault();
                        break;
                    case SubscriptionTypeEnum.Others:
                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                            ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                            : versions?.AsEnumerable()
                                .OrderBy(vp => Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice))
                                .ThenBy(vp => vp.YearlyNetPrice)
                                //  .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                .FirstOrDefault();
                        break;
                    case SubscriptionTypeEnum.All:
                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                            ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                            : versions?.AsEnumerable()
                                .OrderBy(vp => Math.Min(Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice), vp.ForeverNetPrice))
                                .ThenBy(vp => vp.ForeverNetPrice)
                                .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                .FirstOrDefault();
                        break;

                    default:
                        return null;
                }




                //switch ((SubscriptionTypeEnum)app.SubscriptionTypeId)
                //{
                //    case SubscriptionTypeEnum.Forever:
                //        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                //                        ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                //                        : versions?.OrderBy(vp => vp.ForeverNetPrice)
                //                                  .FirstOrDefault();
                //        break;
                //    case SubscriptionTypeEnum.Others:
                //        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                //                        ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                //                        : versions?.OrderBy(vp => vp.MonthlyNetPrice)
                //                                  .FirstOrDefault();
                //        break;
                //    default:
                //        return null;
                //}

                //if ((SubscriptionTypeEnum)app.SubscriptionTypeId == SubscriptionTypeEnum.Forever)
                //    return _mapper.Map<ForeverPriceDetailsDto>(minVersionPrice);
                //else
                //    return _mapper.Map<MonthlyPriceDetailsDto>(minVersionPrice);




                if ((SubscriptionTypeEnum)app.SubscriptionTypeId == SubscriptionTypeEnum.Forever)
                    return _mapper.Map<ForeverPriceDetailsDto>(minVersionPrice);
                if ((SubscriptionTypeEnum)app.SubscriptionTypeId == SubscriptionTypeEnum.Others)
                    return minVersionPrice.MonthlyNetPrice < minVersionPrice.YearlyNetPrice ? _mapper.Map<MonthlyPriceDetailsDto>(minVersionPrice) : _mapper.Map<YearlyPriceDetailsDto>(minVersionPrice);

                var minNetPrice = Math.Min(Math.Min(minVersionPrice.MonthlyNetPrice, minVersionPrice.ForeverNetPrice), minVersionPrice.YearlyNetPrice);
                // Determine which price and discount to use based on the minimum net price

                if (minNetPrice == minVersionPrice.MonthlyNetPrice)
                {
                    return _mapper.Map<MonthlyPriceDetailsDto>(minVersionPrice);
                }

                if (minNetPrice == minVersionPrice.YearlyNetPrice)
                {
                    return _mapper.Map<YearlyPriceDetailsDto>(minVersionPrice);

                }

                // minNetPrice == src.ForeverNetPrice
                return _mapper.Map<ForeverPriceDetailsDto>(minVersionPrice);


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public VersionPriceAllDetailsDto GetMinimumVersionAllPrices(int versionId, int? countryId = null, int? priceLevelId = null)
        {
            try
            {

                var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;
                var versions = _versionPriceRepository
                           .WhereIf(vp => vp.CountryCurrency != null && vp.CountryCurrency.Id == countryCurrencyId, countryCurrencyId.HasValue && countryCurrencyId.Value > 0)
                           .WhereIf(vp => vp.PriceLevelId == priceLevelId, priceLevelId.HasValue && priceLevelId.Value > 0)
                           .Where(vp => vp.VersionId == versionId);

                var app = versions.Select(v => v.Version.Application).FirstOrDefault();

                if (app is null)
                    return null;

                var minVersionPrice = new VersionPrice();

                switch ((SubscriptionTypeEnum)app.SubscriptionTypeId)
                {
                    case SubscriptionTypeEnum.Forever:



                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                       ? versions.FirstOrDefault(vp => vp.PriceLevelId == priceLevelId.Value)
                                       : versions?
                                           .OrderBy(vp => vp.ForeverNetPrice)
                                           .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                           .FirstOrDefault();
                        break;
                    case SubscriptionTypeEnum.Others:
                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                       ? versions.FirstOrDefault(vp => vp.PriceLevelId == priceLevelId.Value)
                                       : versions?.AsEnumerable()
                                           .OrderBy(vp => Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice))
                                           .ThenBy(vp => vp.YearlyNetPrice)
                                           //  .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                           .FirstOrDefault();
                        break;

                    case SubscriptionTypeEnum.All:
                        minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                       ? versions.FirstOrDefault(vp => vp.PriceLevelId == priceLevelId.Value)
                                       : versions?.AsEnumerable()
                                           .OrderBy(vp => Math.Min(Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice), vp.ForeverNetPrice))
                                           .ThenBy(vp => vp.ForeverNetPrice)
                                           .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                           .FirstOrDefault();
                        break;


                    default:
                        return null;
                }

                return new VersionPriceAllDetailsDto
                {
                    Forever = minVersionPrice != null ? _mapper.Map<ForeverPriceDetailsDto>(minVersionPrice) : null,
                    Monthly = minVersionPrice != null ? _mapper.Map<MonthlyPriceDetailsDto>(minVersionPrice) : null,
                    Yearly = minVersionPrice != null ? _mapper.Map<YearlyPriceDetailsDto>(minVersionPrice) : null,
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Dictionary<SubscriptionTypeEnum, VersionPrice> GetApplicationMinimumVersionPriceHelper(int appId, int? countryId = null, int? priceLevelId = null)
        {
            var priceDictionary = new Dictionary<SubscriptionTypeEnum, VersionPrice>();

            var countryCurrencyId = _countryBLL.GetByCountryIdOrDefaultAsync(countryId).GetAwaiter().GetResult()?.Data?.Id;
            var versions = _versionPriceRepository
                                .WhereIf(vp => vp.CountryCurrencyId == countryCurrencyId, countryCurrencyId.HasValue)
                                .WhereIf(vp => vp.PriceLevelId == priceLevelId, priceLevelId.HasValue && priceLevelId.Value > 0)
                                .Where(vp => vp.Version != null && vp.Version.ApplicationId == appId);
            var app = versions.Select(v => v.Version.Application).FirstOrDefault();

            if (app is null)
                return null;

            var minVersionPrice = new VersionPrice();

            switch ((SubscriptionTypeEnum)app.SubscriptionTypeId)
            {
                case SubscriptionTypeEnum.Forever:
                    minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                    ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                                    : versions?
                                   .OrderBy(vp => vp.ForeverNetPrice)
                                   .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                   .FirstOrDefault();
                    break;
                case SubscriptionTypeEnum.Others:
                    minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                    ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                                    : versions?.AsEnumerable()
                                        .OrderBy(vp => Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice))
                                   .ThenBy(vp => vp.YearlyNetPrice)
                                   //  .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                   .FirstOrDefault();
                    break;
                case SubscriptionTypeEnum.All:
                    minVersionPrice = priceLevelId.HasValue && priceLevelId.Value > 0
                                      ? versions?.FirstOrDefault(vp => vp.Id == priceLevelId.Value)
                                      : versions?.AsEnumerable()
                                          .OrderBy(vp => Math.Min(Math.Min(vp.MonthlyNetPrice, vp.YearlyNetPrice), vp.ForeverNetPrice))
                                          .ThenBy(vp => vp.ForeverNetPrice)
                                          .ThenBy(vp => vp.PriceLevel.NumberOfLicenses)
                                          .FirstOrDefault();
                    break;

                default:
                    return null;
            }

            priceDictionary.Add((SubscriptionTypeEnum)app.SubscriptionTypeId, minVersionPrice);

            return priceDictionary;
        }

        #endregion

        #region Version

        private bool IsVersionNameExist(JsonLanguageModel name, int applicationId, LangEnum lang = LangEnum.Default, int? excludedId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
              ? _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any()
              : _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any();
        }
        private bool IsVersionTitleExist(JsonLanguageModel name, int applicationId, LangEnum lang = LangEnum.Default, int? excludedId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
              ? _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Title, "$.default") == name.Default.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any()
              : _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Title, "$.ar") == name.Ar.Trim() && a.ApplicationId == applicationId)
                             .WhereIf(a => a.Id != excludedId, excludedId.HasValue).Any();
        }
        private void UpdateOldHiglightedVersionToNotHighLighted(int ApplicationId, int? versionId = null)
        {
            var oldHighlighted = _versionRepository
                .DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != versionId, versionId.HasValue)
                .Where(x => x.ApplicationId == ApplicationId && x.IsHighlightedVersion).ToList();
            oldHighlighted.ForEach(c =>
            {
                c.IsHighlightedVersion = false;

            });
        }

        private Version GetFirstApplicationVersion(int applicationId, int? versionId = null)
        {
            return _versionRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(x => x.Id != versionId, versionId.HasValue).Where(f => f.ApplicationId == applicationId).OrderBy(f => f.Id).FirstOrDefault();
        }
        private int GetActiveVersionsCount(int applicationId, int? versionId)
        {
            return
                _versionRepository
                //.DisableFilter(nameof(DynamicFilters.IsActive))
                .WhereIf(x => x.Id != versionId, versionId.HasValue)
                .Where(f => f.ApplicationId == applicationId && f.IsActive).Count();

        }
        #endregion

        #region VersionPrice
        public bool IsVersionPriceAlreadExsist(CreateVersionPriceInputDto dto, int? Id = null)
        {
            return _versionPriceRepository.DisableFilter(nameof(DynamicFilters.IsActive)).WhereIf(pl => pl.Id != Id, Id.HasValue).Any(s => s.VersionId == dto.VersionId && s.CountryCurrencyId == dto.CountryCurrencyId && s.PriceLevelId == dto.PriceLevelId);
        }
        public List<RetrieveVersionPrice> GetApplicationVersionPrices(int versionId, int? countryCurrencyId = null)
        {
            try
            {

                var versionPrices = _versionPriceRepository.Where(x => x.CountryCurrencyId == countryCurrencyId &&
                                                                                                x.VersionId == versionId)
                                                                          .ToList();

                if (versionPrices is null)
                    return null;

                return _mapper.Map<List<RetrieveVersionPrice>>(versionPrices);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #endregion
    }
}
