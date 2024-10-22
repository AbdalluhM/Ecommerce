using AutoMapper;

using FluentValidation;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Applications.Modules;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionModules;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Applications.Modules
{
    public class VersionModuleBLL : BaseBLL, IVersionModuleBLL
    {

        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<Module> _moduleRepository;
        IRepository<VersionModule> _versionModuleRepository;
        #endregion
        #region Constructor
        public VersionModuleBLL( IMapper mapper, IUnitOfWork unitOfWork, IRepository<Module> moduleRepository, IRepository<VersionModule> versionModuleRepository, IRepository<Application> applicationRepository ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _moduleRepository = moduleRepository;
            _versionModuleRepository = versionModuleRepository;
        }


        #endregion
        #region Module

        #region UnAsignedModule
        public async Task<IResponse<List<GetUnAsignedModuleOutputDto>>> GetAllUnAsignedModuleAsync(GetApplicationByIdInputDto inputDto)
        {

            var output = new Response<List<GetUnAsignedModuleOutputDto>>();
            try
            {
                //Input Valiation
                var validator = new GetUnAsignedModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Get all versionmodule Ids
                var excludedId = _versionModuleRepository.Where(x => x.Version.ApplicationId == inputDto.ApplicationId).Select(x => x.ModuleId).ToList();

                var result = _mapper.Map<List<GetUnAsignedModuleOutputDto>>(_moduleRepository
                    .Where(x => !excludedId.Contains(x.Id)))
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

        public async Task<IResponse<GetVersionModuleOutputDto>> CreateAsync( CreateVersionModuleInputDto inputDto )
        {
            var output = new Response<GetVersionModuleOutputDto>();

            try
            {
                //Input Validations
                var validator = new CreateVersionModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Business Validations
                var versions = inputDto.Versions.Select(x => x.VersionId).ToList();
                var alreadyExistVersions = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId).Select(x => x.VersionId).ToList();
                var newVersions = versions.Except(alreadyExistVersions);

                #region Business Validations Comment

                ////1-check if all versionIds already assigned to Module
                //if (!IsModuleAlreadyAssignedToVersions(inputDto.ModuleId, inputDto.Versions.Select(x => x.VersionId).ToList()))
                //    output.AppendError(MessageCodes.NotFound, nameof(CountryCurrency));


                //if (!output.IsSuccess)
                //    return output.CreateResponse();

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var ModuleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ModuleId);
                // 3- Check if Module Entity Exists
                if (ModuleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                #endregion

                //Business 
                //map entity
                var mappedInput = _mapper.Map<List<VersionModule>>(inputDto.Versions.Where(x => newVersions.Contains(x.VersionId)).ToList());

                _versionModuleRepository.Add(mappedInput);
               
                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionModuleOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedModuleVersionsDto>>(mappedInput);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        private bool IsModuleAlreadyAssignedToVersions( int ModuleId, List<int> inputIds )
        {
            var s = _versionModuleRepository/*.DisableFilter(nameof(DynamicFilters.IsActive))*/.Where(x => x.ModuleId == ModuleId).Select(x => x.VersionId).Intersect(inputIds).Count();
            return _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == ModuleId).Select(x => x.VersionId).Intersect(inputIds).Count() == inputIds.Count();

        }
        public async Task<IResponse<GetVersionModuleOutputDto>> UpdateAysnc( UpdateVersionModuleInputDto inputDto )
        {
            var output = new Response<GetVersionModuleOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateVersionModuleInputDtoValidator().Validate(inputDto);
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

                var ModuleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ModuleId);
                // 3- Check if Module Entity Exists
                if (ModuleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                #region comment
                //solu 1
                ////HardDelete versionModule records
                //_versionModuleRepository.Delete(x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId);
                ////Add/Assign Versions to Module
                //var mappedInput = _mapper.Map<List<VersionModule>>(inputDto.Versions);
                //_versionModuleRepository.Add(mappedInput);

                //solu 2
                //var versionAddonsDB = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                ////set id for existing records (because it is not get from front)
                //mappedInput.ForEach(x =>
                //{
                //    x.ModuleId = inputDto.ModuleId;
                //    var item = versionAddonsDB.Where(va => va.ModuleId == x.ModuleId && va.VersionId == x.VersionId).FirstOrDefault();
                //    if (item != null)
                //    {
                //        x.Id = item.Id;
                //    }

                //});
                #endregion


                var mappedInput = _mapper.Map<List<VersionModule>>(inputDto.Versions);
              

                //Update CrossTable VersionFeature
                _versionModuleRepository.UpdateCrossTable(mappedInput, x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId, "VersionModules", isSoftDelete: true, currentUser: inputDto.ModifiedBy);

                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionModuleOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedModuleVersionsDto>>(mappedInput);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<bool>> DeleteAsync( DeleteApplicatinModuleInputDto inputDto )
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteVersionModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }



                //Business Validations
                //1-
                var entityList = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                // 1- Check if Entity Exists
                if (entityList == null || entityList?.Count() == 0)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionModule));

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var ModuleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ModuleId);
                // 3- Check if Module Entity Exists
                if (ModuleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));


                bool result = true;

                foreach (var entity in entityList)
                {
                    //  2 - Check if entity has references
                    var checkDto = EntityHasReferences(entity.Id, _versionModuleRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(VersionModule)));
                    if (checkDto.HasChildren == 0)
                    {
                        //   soft delete entity 
                        entity.IsDeleted = true;
                        entity.ModifiedBy = inputDto.ModifiedBy;
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
        public async Task<IResponse<GetVersionModuleOutputDto>> GetByIdAsync( GetVersionModuleInputDto inputDto )
        {
            var output = new Response<GetVersionModuleOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetVersionModuleInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entityList = _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.ModuleId == inputDto.ModuleId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();
                //1-
                if (entityList == null || entityList.Count() == 0)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var ModuleEntity = _moduleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ModuleId);
                // 3- Check if Module Entity Exists
                if (ModuleEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Module));

                var result = _mapper.Map<GetVersionModuleOutputDto>(entityList);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<List<GetVersionModuleOutputDto>>> GetAllAsync( GetAllVersionModuleInputDto inputDto )
        {
            var output = new Response<List<GetVersionModuleOutputDto>>();
            try
            {
                var result = new List<GetVersionModuleOutputDto>();
                result = _versionModuleRepository
                    .DisableFilter(nameof(DynamicFilters.IsActive))
                    .Where(x => x.Version.ApplicationId == inputDto.ApplicationId)
                    .AsEnumerable()
                    .GroupBy(x => x.ModuleId)
                    .Select(a => new GetVersionModuleOutputDto
                        {
                            ApplicationId = a.FirstOrDefault().Version.ApplicationId,
                            ModuleId = a.Key,
                            IsActive = a.Any(s=>!s.IsActive)? false : true, //TODO:Refactor this
                            ApplicationName = a.FirstOrDefault().Version.Application.Name,
                            ModuleName = a.FirstOrDefault().Module.Name,
                            Versions = _mapper.Map<List<GetAssignedModuleVersionsDto>>(a.ToList())
                        })
                     .OrderBy(x => x.ModuleId)
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
        public async Task<IResponse<PagedResultDto<GetVersionModuleOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto )
        {
            var output = new Response<PagedResultDto<GetVersionModuleOutputDto>>();
            #region Comment
            //var result = GetPagedList<GetVersionModuleOutputDto, VersionModule, int>(pagedDto: pagedDto, repository: _versionModuleRepository, orderExpression: x => x.Id,
            //  searchExpression: x =>  x.Version.ApplicationId == pagedDto.ApplicationId, 
            //    sortDirection: nameof(SortingDirection.ASC),
            //   disableFilter: true);
            //return output.CreateResponse(result);
            #endregion
            //keep original skip & max Result Counts
            int originalSkip = pagedDto.SkipCount;
            int originalMaxResultCount = pagedDto.MaxResultCount;
            //update skip & max Result Counts to search in cross table before group by
            pagedDto.SkipCount = 0;
            pagedDto.MaxResultCount = int.MaxValue;

            ////get active modules only
            //var activeModuleIds = _moduleRepository.GetAll().Select(x => x.Id).ToList();

            var data = GetPagedList<GetAssignedModuleVersionsDto, VersionModule, int>(
              pagedDto: pagedDto,
              repository: _versionModuleRepository, 
              orderExpression: x => x.Id,
              searchExpression: x =>/* activeModuleIds.Contains(x.ModuleId) && */ x.Version.ApplicationId == pagedDto.ApplicationId,
              sortDirection: pagedDto.SortingDirection,
              disableFilter: true,
              excluededColumns:null,
               x => x.Version.Application, x=>x.Module
              );
            var result = new PagedResultDto<GetVersionModuleOutputDto> ();
            var query = data.Items
                    .GroupBy(x => x.ModuleId)
                    .Select(a => new GetVersionModuleOutputDto
                    {
                        ApplicationId = pagedDto.ApplicationId,
                        ModuleId = a.Key,
                        IsActive = a.Any(s => !s.IsActive) ? false : true, //TODO:Refactor this
                        ApplicationName = a.FirstOrDefault().ApplicationName,
                        ModuleName = a.FirstOrDefault().ModuleName,
                        Versions = _mapper.Map<List<GetAssignedModuleVersionsDto>>(a.ToList())
                    });

            //make pagination  original skip & max Result & ordering Counts on the grouped result 
            result.Items = query.Skip(originalSkip).Take(originalMaxResultCount).OrderBy(x => x.ModuleId).ToList();
            result.TotalCount = query.Count();
            return output.CreateResponse(result);
        }



        #endregion
        #region Totals
        public int GetVersionModulesCount( int applicationId )
        {
           var versionIds =  _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x=>x.Version.ApplicationId == applicationId).Select(x => x.VersionId).Distinct().ToList();
           return _versionModuleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x=> versionIds.Contains(x.VersionId)).GroupBy(x=>x.ModuleId).Count();
        }
        #endregion
        #endregion

    }
}

