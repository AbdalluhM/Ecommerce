using AutoMapper;

using FluentValidation;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Applications.Features;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Applications.Features
{
    public class VersionFeatureBLL : BaseBLL, IVersionFeatureBLL
    {

        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<Feature> _featureRepository;
        IRepository<VersionFeature> _versionFeatureRepository;
        IRepository<Core.Entities.Version> _versionRepository;
        IRepository<ViewApplicationVersionFeature> _applicationVersionFeatureRepository;

        #endregion
        #region Constructor
        public VersionFeatureBLL( IMapper mapper, IUnitOfWork unitOfWork, IRepository<Feature> featureRepository, IRepository<VersionFeature> versionFeatureRepository, IRepository<Application> applicationRepository, IRepository<ViewApplicationVersionFeature> applicationVersionFeatureRepository, IRepository<Core.Entities.Version> versionRepository ) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _featureRepository = featureRepository;
            _versionFeatureRepository = versionFeatureRepository;
            _applicationVersionFeatureRepository = applicationVersionFeatureRepository;
            _versionRepository = versionRepository;
        }


        #endregion
        #region Feature
        #region UnAsignedFeature
        public async Task<IResponse<List<GetUnAsignedFeatureOutputDto>>> GetAllUnAsignedFeatureAsync( GetApplicationByIdInputDto inputDto )
        {

            var output = new Response<List<GetUnAsignedFeatureOutputDto>>();
            try
            {
                //Input Validation
                var validator = new GetUnAsignedFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Get all versionFeature Ids
                var excludedId = _versionFeatureRepository.Where(x => x.Version.ApplicationId == inputDto.ApplicationId)
                    .Select(x => x.FeatureId).ToList();
                var result = _mapper.Map<List<GetUnAsignedFeatureOutputDto>>(_featureRepository
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

        public async Task<IResponse<bool>> CreateAsync( CreateVersionFeatureInputDto inputDto )
        {
            var output = new Response<bool>();

            try
            {
                //Input Validations
                var validator = new CreateVersionFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);

                }
                //Business Validations
                var versions = inputDto.Versions.Select(x => x.VersionId).ToList();
                var alreadyExistVersions = _versionFeatureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId).Select(x => x.VersionId).ToList();
                var newVersions = versions.Except(alreadyExistVersions);

                #region Business Validations Comment

                ////1-check if all versionIds already assigned to feature
                //if (!IsFeatureAlreadyAssignedToVersions(inputDto.FeatureId, inputDto.Versions.Select(x => x.VersionId).ToList()))
                //    output.AppendError(MessageCodes.NotFound, nameof(CountryCurrency));


                //if (!output.IsSuccess)
                //    return output.CreateResponse();
                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var featureEntity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.FeatureId);
                // 3- Check if feature Entity Exists
                if (featureEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));
                #endregion

                //Business 
                //map entity
                var mappedInput = _mapper.Map<List<VersionFeature>>(inputDto.Versions.Where(x => newVersions.Contains(x.VersionId)).ToList());

                _versionFeatureRepository.Add(mappedInput);

                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionFeatureOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedFeatureVersionsDto>>(mappedInput);
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        private bool IsFeatureAlreadyAssignedToVersions( int featureId, List<int> inputIds )
        {
            var s = _versionFeatureRepository/*.DisableFilter(nameof(DynamicFilters.IsActive))*/.Where(x => x.FeatureId == featureId).Select(x => x.VersionId).Intersect(inputIds).Count();
            return _versionFeatureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.FeatureId == featureId).Select(x => x.VersionId).Intersect(inputIds).Count() == inputIds.Count();

        }
        public async Task<IResponse<GetVersionFeatureOutputDto>> UpdateAysnc( UpdateVersionFeatureInputDto inputDto )
        {
            var output = new Response<GetVersionFeatureOutputDto>();

            try
            {
                //Input Validation
                var validator = new UpdateVersionFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                #region Business Validations Comment
                ////Business Validations
                ////1-check if all versionIds already assigned to feature
                //if (!IsFeatureAlreadyAssignedToVersions(inputDto.FeatureId, inputDto.Versions.Select(x => x.VersionId).ToList()))
                //    output.AppendError(MessageCodes.NotFound, nameof(CountryCurrency));


                //if (!output.IsSuccess)
                //    return output.CreateResponse();
                #endregion

                //Business
                //BusinessValidation
                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var featureEntity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.FeatureId);
                // 3- Check if feature Entity Exists
                if (featureEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));

                #region comment
                ///solu 1

                ////HardDelete versionFeature records
                //_versionFeatureRepository.Delete(x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId);
                ////Add/Assign Versions to Feature
                //var mappedInput = _mapper.Map<List<VersionFeature>>(inputDto.Versions);
                //_versionFeatureRepository.Add(mappedInput);
                //////////
                ///solu 2
                //var mappedInput = _mapper.Map<List<VersionFeature>>(inputDto.Versions);

                // //var versionAddonsDB = _versionFeatureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                ////set id for existing records (because it is not get from front)
                //mappedInput.ForEach(x =>
                //{
                //    x.FeatureId = inputDto.FeatureId;
                //    var item = versionAddonsDB.Where(va => va.FeatureId == x.FeatureId && va.VersionId == x.VersionId).FirstOrDefault();
                //    if (item != null)
                //    {
                //        x.Id = item.Id;
                //    }

                //});

                #endregion

                var mappedInput = _mapper.Map<List<VersionFeature>>(inputDto.Versions);

                //Update CrossTable VersionFeature
                _versionFeatureRepository.UpdateCrossTable(mappedInput, x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId, "VersionFeatures", isSoftDelete: true, currentUser: inputDto.ModifiedBy);

                _unitOfWork.Commit();
                var result = _mapper.Map<GetVersionFeatureOutputDto>(inputDto);
                result.Versions = _mapper.Map<List<GetAssignedFeatureVersionsDto>>(mappedInput);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        public async Task<IResponse<bool>> DeleteAsync( DeleteApplicatinFeatureInputDto inputDto )
        {
            var output = new Response<bool>();

            try
            {
                //Input Validation
                var validator = new DeleteVersionFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }



                //Business Validations
                //1-
                var entityList = _versionFeatureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();

                // 1- Check if Entity Exists
                if (entityList == null || entityList?.Count() == 0)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(VersionFeature));

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var featureEntity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.FeatureId);
                // 3- Check if feature Entity Exists
                if (featureEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));


                bool result = true;

                foreach (var entity in entityList)
                {
                    //  2 - Check if entity has references
                    var checkDto = EntityHasReferences(entity.Id, _versionFeatureRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(VersionFeature)));
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
        public async Task<IResponse<GetVersionFeatureOutputDto>> GetByIdAsync( GetVersionFeatureInputDto inputDto )
        {
            var output = new Response<GetVersionFeatureOutputDto>();

            try
            {
                //Input Validation
                var validator = new GetVersionFeatureInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                var entityList = _versionFeatureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).Where(x => x.FeatureId == inputDto.FeatureId && x.Version.ApplicationId == inputDto.ApplicationId).ToList();
                //1-
                if (entityList == null || entityList.Count() == 0)
                    return output.CreateResponse(MessageCodes.FailedToFetchData);

                var applicationEntity = _applicationRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                // 2- Check if application Entity Exists
                if (applicationEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Application));

                var featureEntity = _featureRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.FeatureId);
                // 3- Check if feature Entity Exists
                if (featureEntity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Feature));

                var result = _mapper.Map<GetVersionFeatureOutputDto>(entityList);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<List<GetVersionFeatureOutputDto>>> GetAllAsync( GetAllVersionFeatureInputDto inputDto )
        {
            var output = new Response<List<GetVersionFeatureOutputDto>>();
            try
            {
                var result = new List<GetVersionFeatureOutputDto>();
                result = _versionFeatureRepository
                    .DisableFilter(nameof(DynamicFilters.IsActive))
                    .Where(x => x.Version.ApplicationId == inputDto.ApplicationId)
                    .AsEnumerable()
                    .GroupBy(x => x.FeatureId)
                    .Select(a => new GetVersionFeatureOutputDto
                    {
                        ApplicationId = inputDto.ApplicationId,// a.FirstOrDefault()?.Version?.ApplicationId ?? ,
                        FeatureId = a.Key,
                        IsActive = a.Any(s => !s.IsActive) ? false : true, //TODO:Refactor this
                        ApplicationName = a.FirstOrDefault()?.Version?.Application.Name,
                        FeatureName = a.FirstOrDefault()?.Feature?.Name,
                        Versions = _mapper.Map<List<GetAssignedFeatureVersionsDto>>(a.ToList())
                    })
                    .OrderBy(x => x.FeatureId)
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
        public async Task<IResponse<PagedResultDto<GetVersionFeatureOutputDto>>> GetPagedListAsync( ApplicationFilteredPagedResult pagedDto )
        {
            var output = new Response<PagedResultDto<GetVersionFeatureOutputDto>>();

            //keep original skip & max Result Counts
            int originalSkip = pagedDto.SkipCount;
            int originalMaxResultCount = pagedDto.MaxResultCount;
            //update skip & max Result Counts to search in cross table before group by
            pagedDto.SkipCount = 0;
            pagedDto.MaxResultCount = int.MaxValue;

            ////get active features only
            //var activeFeatureIds = _featureRepository.GetAll().Select(x => x.Id).ToList();

            var data = GetPagedList<GetAssignedFeatureVersionsDto, VersionFeature, int>(
                pagedDto: pagedDto,
                repository: _versionFeatureRepository,
                orderExpression: x => x.Id,
                searchExpression: x => /*activeFeatureIds.Contains(x.FeatureId) && */x.Version.ApplicationId == pagedDto.ApplicationId,
                sortDirection: pagedDto.SortingDirection,
                disableFilter: true,
                excluededColumns: null,
                x=>x.Version.Application,x=>x.Feature);
            var result = new PagedResultDto<GetVersionFeatureOutputDto>();
            var query = data.Items
                    .GroupBy(x => x.FeatureId)
                    .Select(a => new GetVersionFeatureOutputDto
                    {
                        ApplicationId = pagedDto.ApplicationId,
                        FeatureId = a.Key,
                        IsActive = true,//a.Any(s => !s.IsActive) ? false : true, //TODO:Refactor this
                        ApplicationName = a.FirstOrDefault().ApplicationName,
                        FeatureName = a.FirstOrDefault().FeatureName,
                        Versions = _mapper.Map<List<GetAssignedFeatureVersionsDto>>(a.ToList())
                    });

            //make pagination  original skip & max Result & ordering Counts on the grouped result 
            result.Items = query.Skip(originalSkip).Take(originalMaxResultCount).OrderBy(x => x.FeatureId).ToList();
            result.TotalCount = query.Count();

            //  var versions = _versionRepository.Where(x => x.ApplicationId == pagedDto.ApplicationId);
            //  var versionIds = versions?.Select(x => x.Id).ToList();
            //  var featureIds = _versionFeatureRepository.Where(x => versionIds.Contains(x.VersionId)).Select(x=>x.FeatureId).ToList();
            //  var featureVesion = GetPagedList<GetAllVersionFeatureOutputDto, Feature, int>(pagedDto: pagedDto, repository:
            //    _featureRepository, orderExpression: x => x.Id,
            //searchExpression: x => featureIds.Contains(x.Id),
            //  sortDirection: nameof(SortingDirection.ASC),
            // disableFilter: false);

            //  featureVesion.Items.ToList().ForEach(x =>
            //  {

            //      x.ApplicationName = version.Application.Name;
            //      x.Versions = _mapper.Map<List<GetVersionAssignFeatureOutputDto>>(_versionFeatureRepository
            //          .Where(x=>featureIds.Contains(x.FeatureId)).Select(x=>x.Version).ToList());
            //  });

            return output.CreateResponse(result);
        }




        #endregion
        #endregion

    }
}

