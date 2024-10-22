using AutoMapper;
using FluentValidation;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Applications.AddOns;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionAddOns;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Applications.AddOns
{
    public class VersionAddOnBLL : BaseBLL, IVersionAddOnBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<AddOn> _addOnRepository;
        IRepository<VersionAddon> _versionAddonRepository;

        #endregion
      
        #region Constructor
        public VersionAddOnBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<AddOn> addOnRepository, IRepository<VersionAddon> versionAddonRepository, IRepository<Application> applicationRepository, IRepository<Core.Entities.Version> versionRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _addOnRepository = addOnRepository;
            _versionAddonRepository = versionAddonRepository;
        }
        #endregion
        
        #region AddOn
     
        #region UnAsignedAddOn
        public async Task<IResponse<List<GetUnAsignedAddOnOutputDto>>> GetAllUnAsignedAddOnAsync(GetApplicationByIdInputDto inputDto)
        {

            var output = new Response<List<GetUnAsignedAddOnOutputDto>>();
            try
            {
                //Input Validation
                //
                var validator = new GetUnAsignedVersionInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Get all versionaddon Ids
                var excludedIds = _versionAddonRepository.Where(x => x.Version.ApplicationId == inputDto.ApplicationId)
                    .Select(x => x.AddonId).ToList();
                var result = _mapper.Map<List<GetUnAsignedAddOnOutputDto>>(_addOnRepository
                    .Where(x => !excludedIds.Contains(x.Id)))
                    .ToList();
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion
        
        #region Basic CRUD 
        public async Task<IResponse<GetVersionAddOnOutputDto>> CreateAsync( CreateVersionAddOnInputDto inputDto )
        {
            var output = new Response<GetVersionAddOnOutputDto>();

            try
            {
                //Input Validations
                var validator = new CreateVersionAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }

                //Business Validations
                var versions = inputDto.Versions.Select(x => x.VersionId).ToList();
                var alreadyExistVersions = _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId).Select(x => x.VersionId).ToList();
                var newVersions = versions.Except(alreadyExistVersions);

                #region Business Validations Comment

                ////1-check if all versionIds already assigned to AddOn
                //if (!IsAddOnAlreadyAssignedToVersions(inputDto.AddOnId, inputDto.Versions.Select(x => x.VersionId).ToList()))
                //    output.AppendError(MessageCodes.NotFound, nameof(CountryCurrency));


                //if (!output.IsSuccess)
                //    return output.CreateResponse();

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var AddOnEntity = _addOnRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.AddOnId);
                // 3- Check if AddOn Entity Exists
                if (AddOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));
                #endregion

                //Business 
                //map entity
                var mappedInput = _mapper.Map<List<VersionAddon>>(inputDto.Versions.Where(x => newVersions.Contains(x.VersionId)).ToList());

                _versionAddonRepository.Add(mappedInput);

                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionAddOnOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedAddOnVersionsDto>>(mappedInput);

                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        private bool IsAddOnAlreadyAssignedToVersions( int AddOnId, List<int> inputIds )
        {
            var s = _versionAddonRepository/*.DisableFilter(nameof(DynamicFilters.IsActive))*/.Where(x => x.AddonId == AddOnId).Select(x => x.VersionId).Intersect(inputIds).Count();
            return _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddonId == AddOnId).Select(x => x.VersionId).Intersect(inputIds).Count() == inputIds.Count();

        }
        public async Task<IResponse<GetVersionAddOnOutputDto>> UpdateAysnc( UpdateVersionAddOnInputDto inputDto )
        {
            var output = new Response<GetVersionAddOnOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateVersionAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business
                //BusinessValidation
                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var AddOnEntity = _addOnRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.AddOnId);
                // 3- Check if AddOn Entity Exists
                if (AddOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                #region comment
                //solu 1
                ////HardDelete VersionAddon records
                //_versionAddonRepository.Delete(x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId);
                ////Add/Assign Versions to AddOn
                //var mappedInput = _mapper.Map<List<VersionAddon>>(inputDto.Versions);
                //_versionAddonRepository.Add(mappedInput);

                //solu2 
                //var versionAddonsDB = _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                ////set id for existing records (because it is not get from front)
                //mappedInput.ForEach(x =>
                //{
                //    x.AddonId = inputDto.AddOnId;
                //    var item = versionAddonsDB.Where(va => va.AddonId == x.AddonId && va.VersionId == x.VersionId).FirstOrDefault();
                //    if (item != null)
                //    {
                //        x.Id = item.Id;
                //    }

                //});
                #endregion

                var mappedInput = _mapper.Map<List<VersionAddon>>(inputDto.Versions);
                

                //Update CrossTable VersionAddon
                _versionAddonRepository.UpdateCrossTable(mappedInput, x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId, "VersionAddons", isSoftDelete: true, currentUser: inputDto.ModifiedBy);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetVersionAddOnOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedAddOnVersionsDto>>(mappedInput);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> DeleteAsync( DeleteApplicatinAddOnInputDto inputDto )
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteVersionAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }



                //Business Validations
                //1-
                var entityList = _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                // 1- Check if Entity Exists
                if (entityList == null || entityList?.Count() == 0)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionAddon));

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var AddOnEntity = _addOnRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.AddOnId);
                // 3- Check if AddOn Entity Exists
                if (AddOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));


                bool result = true;

                foreach (var entity in entityList)
                {
                    //  2 - Check if entity has references
                    var checkDto = EntityHasReferences(entity.Id, _versionAddonRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(VersionAddon)));
                    if (checkDto.HasChildren == 0)
                    {
                        //   soft delete entity 
                        entity.IsDeleted = true;
                        //entity.ModifiedBy = inputDto.ModifiedBy;
                        entity.ModifiedDate = DateTime.UtcNow;

                    }
                    else
                    {
                        result = false;
                    }
                }
                if (result)
                {
                    _unitOfWork.Commit();
                    return await Task.FromResult(output.CreateResponse(true));

                }
                else
                    // reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

            }

            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }

        }
        public async Task<IResponse<GetVersionAddOnOutputDto>> GetByIdAsync( GetVersionAddOnInputDto inputDto )
        {
            var output = new Response<GetVersionAddOnOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetVersionAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entityList = _versionAddonRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.AddonId == inputDto.AddOnId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();
                //1-
                if (entityList == null || entityList.Count() == 0)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var AddOnEntity = _addOnRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.AddOnId);
                // 3- Check if AddOn Entity Exists
                if (AddOnEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(AddOn));

                var result = _mapper.Map<GetVersionAddOnOutputDto>(entityList);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<List<GetVersionAddOnOutputDto>>> GetAllAsync( GetApplicationByIdInputDto inputDto )
        {
            var output = new Response<List<GetVersionAddOnOutputDto>>();
            try
            {
                var result = new List<GetVersionAddOnOutputDto>();
                result = _versionAddonRepository
                    .DisableFilter(nameof(DynamicFilters.IsActive))
                    .Where(x => x.Version.ApplicationId == inputDto.ApplicationId)
                    .AsEnumerable()
                    .GroupBy(x => x.AddonId)
                    .Select(a => new GetVersionAddOnOutputDto
                    {
                        ApplicationId = a.FirstOrDefault().Version.ApplicationId,
                        AddOnId = a.Key,
                        IsActive = a.Any(s => !s.IsActive) ? false : true, //TODO:Refactor this
                        ApplicationName = a.FirstOrDefault().Version.Application.Name,
                        AddOnName = a.FirstOrDefault().Addon.Name,
                        Versions = _mapper.Map<List<GetAssignedAddOnVersionsDto>>(a.ToList())
                    })
                    .OrderBy(x => x.AddOnId)
                    .ToList();

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
        public async Task<IResponse<PagedResultDto<GetVersionAddOnOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto )
        {
            var output = new Response<PagedResultDto<GetVersionAddOnOutputDto>>();

            //keep original skip & max Result Counts
            int originalSkip = pagedDto.SkipCount;
            int originalMaxResultCount = pagedDto.MaxResultCount;
            //update skip & max Result Counts to search in cross table before group by
            pagedDto.SkipCount = 0;
            pagedDto.MaxResultCount = int.MaxValue;

            ////get active addOns only
            //var activeAddOnsIds = _addOnRepository.GetAll().Select(x => x.Id).ToList();

            var data = GetPagedList<GetAssignedAddOnVersionsDto, VersionAddon, int>(
                pagedDto: pagedDto,
                repository: _versionAddonRepository,
                orderExpression: x => x.Id,
                searchExpression: x => /*activeAddOnsIds.Contains(x.AddonId) && */x.Version.ApplicationId == pagedDto.ApplicationId,
                sortDirection: pagedDto.SortingDirection,
                disableFilter: true,
                excluededColumns: null,
                x=>x.Addon);
            var result = new PagedResultDto<GetVersionAddOnOutputDto>();
            var query = data.Items
                    .GroupBy(x => x.AddOnId)
                    .Select(a => new GetVersionAddOnOutputDto
                    {
                        ApplicationId = pagedDto.ApplicationId,
                        AddOnId = a.Key,
                        IsActive = a.Any(s => !s.IsActive) ? false : true, //TODO:Refactor this
                        ApplicationName = a.FirstOrDefault().ApplicationName,
                        AddOnName = a.FirstOrDefault().AddOnName,
                        Versions = _mapper.Map<List<GetAssignedAddOnVersionsDto>>(a.ToList())
                    });

            //make pagination  original skip & max Result & ordering Counts on the grouped result 
            result.Items = query.Skip(originalSkip).Take(originalMaxResultCount).OrderBy(x => x.AddOnId).ToList();
            result.TotalCount = query.Count();

            return output.CreateResponse(result);
        }



        #endregion
        
        #endregion
    }
}

