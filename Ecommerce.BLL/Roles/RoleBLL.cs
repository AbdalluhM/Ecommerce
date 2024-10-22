using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Roles.RoleValidator;
using static Ecommerce.DTO.Roles.RoleDto;
using Action = Ecommerce.Core.Entities.Action;

namespace Ecommerce.BLL.Roles
{
    public class RoleBLL : BaseBLL, IRoleBLL
    {
        #region Fields

        IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        IRepository<Role> _roleRepository;
        IRepository<Page> _PageRepository;
        IRepository<Action> _ActionRepository;
        IRepository<PageAction> _pageActionRepository;
        IRepository<RolePageAction> _rolePageActionRepository;
        private readonly IRealtimeNotificationBLL _realtimeNotificationBLL;
        #endregion

        public RoleBLL(IMapper mapper,
                       IRepository<Role> roleRepository,
                       IRepository<RolePageAction> rolePageActionRepository,
                       IUnitOfWork unitOfWork,
                       IRepository<Page> pageRepository,
                       IRepository<Action> actionRepository,
                       IRepository<PageAction> pageActionRepository,
                       IRealtimeNotificationBLL realtimeNotificationBLL)
            : base(mapper)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _rolePageActionRepository = rolePageActionRepository;
            _unitOfWork = unitOfWork;
            _PageRepository = pageRepository;
            _ActionRepository = actionRepository;
            _pageActionRepository = pageActionRepository;
            _realtimeNotificationBLL = realtimeNotificationBLL;
        }

        public async Task<IResponse<GetRoleOutputDto>> CreateRoleAsync(CreateRoleInputDto inputDto)
        {
            var output = new Response<GetRoleOutputDto>();
            try
            {
                //input validation
                var validation = await new CreateRoleInputDtoValidator().ValidateAsync(inputDto);

                if (!validation.IsValid)
                    return output.CreateResponse(validation.Errors);

                //Business Validation
                //check on (name and title)
                if (IsUniqueRoleName(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(inputDto.Name));
                if (IsUniqueRoleName(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(inputDto.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();


                //Add role
                var role = _roleRepository.Add(new Role
                {
                    Name = inputDto.Name,
                    IsActive = inputDto.IsActive,
                    CreatedBy = inputDto.CreatedBy,
                    CreateDate = DateTime.UtcNow
                });
                _unitOfWork.Commit();
                foreach (var rolePageAction in inputDto.RolePageAction)
                {
                    if (rolePageAction.PageId == 0 || !rolePageAction.ActionIds.Any())
                        continue;

                    var page = await _PageRepository.GetAsync(x => x.Id == rolePageAction.PageId);
                    if (page == null)
                        return output.CreateResponse(MessageCodes.NotFound, nameof(Page));
                    var pageActionIds = await _pageActionRepository.Where(x => x.PageId == rolePageAction.PageId &&
                                                                         rolePageAction.ActionIds.Contains(x.ActionId)).Select(x => x.Id).ToListAsync();

                    foreach (var pageActionId in pageActionIds)
                    {
                        _rolePageActionRepository.Add(new RolePageAction
                        {
                            PageActionId = pageActionId,
                            RoleId = role.Id,
                            CreatedBy = inputDto.CreatedBy,
                            CreateDate = DateTime.UtcNow
                        });
                    }
                }
                _unitOfWork.Commit();
                return output.CreateResponse(_mapper.Map<GetRoleOutputDto>(role));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<GetRoleOutputDto>> UpdateRoleAsync(UpdateRoleInputDto inputDto)
        {
            var output = new Response<GetRoleOutputDto>();
            try
            {
                //input validation
                var validation = await new UpdateRoleInputDtoValidator().ValidateAsync(inputDto);

                var role = await _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefaultAsync(x => x.Id == inputDto.RoleId);
                if (role == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Role));
                //Business Validations
                // 1- Check if Already Exists  (Name)
                if (IsUniqueRoleName(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Default, inputDto.RoleId))
                    output.AppendError(MessageCodes.AlreadyExistsEn, nameof(Role.Name));
                if (IsUniqueRoleName(GetJsonLanguageModelOrNull(inputDto.Name), LangEnum.Ar, inputDto.RoleId))
                    output.AppendError(MessageCodes.AlreadyExistsAr, nameof(Role.Name));

                if (!output.IsSuccess)
                    return output.CreateResponse();

                var actions = inputDto.RolePageAction.Select(x => x.ActionIds).ToList();

                if (!actions.Any(x => x.Count() > 0))
                    return output.CreateResponse(MessageCodes.PageNotSelected);



                role.Name = inputDto.Name;
                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = inputDto.ModifiedBy;
                role.IsActive = inputDto.IsActive;
                //rolepageaction db
                var rolePageActions = await _rolePageActionRepository.Where(x => x.RoleId == role.Id).ToListAsync();
                //new rolePageAction
                var newRolePageActions = new List<RolePageAction>();
                foreach (var rolePageAction in inputDto.RolePageAction)
                {
                    var pageActionIds = await _pageActionRepository.Where(x => x.PageId == rolePageAction.PageId &&
                                                                         rolePageAction.ActionIds.Contains(x.ActionId)).Select(x => x.Id).ToListAsync();

                    foreach (var pageActionId in pageActionIds)
                    {
                        newRolePageActions.Add(new RolePageAction
                        {
                            PageActionId = pageActionId,
                            RoleId = role.Id,
                            CreatedBy = inputDto.CreatedBy,
                            CreateDate = DateTime.UtcNow,
                            ModifiedBy = inputDto.ModifiedBy,
                            ModifiedDate = DateTime.UtcNow
                        });

                    }

                }
                foreach (var newRolePageAction in newRolePageActions)
                {
                    var item = rolePageActions.Where(x => x.PageActionId == newRolePageAction.PageActionId).FirstOrDefault();
                    if (item != null)
                    {
                        newRolePageAction.Id = item.Id;
                    }
                }
                _rolePageActionRepository.UpdateCrossTable(newRolePageActions, x => x.RoleId == role.Id, "RolePageActions");
                _unitOfWork.Commit();

                //Notifiy to all members
                await _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                {
                    Channel = HubChannelEnum.RefreshTokenChannel,
                    RecieverType = NotificationRecieverTypeEnum.Employee,
                    Recievers = role.Employees.Select(x => x.Id).ToList(),
                    Notification = null
                });

                return output.CreateResponse(_mapper.Map<GetRoleOutputDto>(role));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<PagedResultDto<GetRoleOutputDto>>> GetAllRolesPagedList(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetRoleOutputDto>>();

            var result = GetPagedList<GetRoleOutputDto, Role, int>(
                pagedDto: pagedDto,
                repository: _roleRepository, orderExpression: x => x.Id,
              searchExpression: x => (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || (!string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                )),
                sortDirection: pagedDto.SortingDirection,/* sortDirection: nameof(SortingDirection.DESC)*/
                      disableFilter: true);
            return output.CreateResponse(result);
        }
        public async Task<IResponse<List<GetRoleOutputDto>>> GetAllRoleAsync()
        {

            var output = new Response<List<GetRoleOutputDto>>();
            try
            {
                var result = _mapper.Map<List<GetRoleOutputDto>>(await _roleRepository.GetAllListAsync());
                return output.CreateResponse(result);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<GetRoleOutputDto>> GetRoleByIdAsync(int id)
        {
            var output = new Response<GetRoleOutputDto>();
            try
            {
                var role = await _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(role));
                var PageActionIds = role.RolePageActions.Select(x => x.PageActionId).ToList();

                var PageIds = _pageActionRepository.Where(x => PageActionIds.Contains(x.Id)).Select(p => p.PageId).Distinct().ToList();

                var result = _mapper.Map<GetRoleOutputDto>(role);
                result.PagesActions = _mapper.Map<List<GetPageActionOutputDto>>(
                    _pageActionRepository.Where(x => PageActionIds.Contains(x.Id)).Select(x => x.Page).Distinct().ToList());
                result.PagesActions.ForEach(x =>
                {
                    x.ActionIds = _pageActionRepository.Where(a => PageActionIds.Contains(a.Id) && a.PageId == x.PageId).Select(x => x.ActionId).ToList();
                });


                return output.CreateResponse(result);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<bool>> DeleteRoleAsync(int Id)
        {
            var output = new Response<bool>();
            try
            {
                var role = await _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefaultAsync(x => x.Id == Id);
                if (role == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(Role));
                if (role.Employees.Any())
                {
                    return output.CreateResponse(MessageCodes.RoleAssign, nameof(Role));
                }
                // 3- Check if Entity has references
                var checkDto = EntityHasReferences(Id, _roleRepository, string.Format(DbSchemas.ReferenceTableName, DbSchemas.Admin, nameof(RolePageAction)));
                if (checkDto.HasChildren == 0)
                {
                    if (role.RolePageActions.Any())
                    {
                        foreach (var item in role.RolePageActions)
                        {
                            //HardDelete rolepageaction
                            _rolePageActionRepository.Delete(item);
                        }
                    }
                    // role soft delete
                    role.IsDeleted = true;
                    _unitOfWork.Commit();

                    //Notifiy to all members
                    await _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Admin, new UserNotificationDto
                    {
                        Channel = HubChannelEnum.RefreshTokenChannel,
                        RecieverType = NotificationRecieverTypeEnum.Employee,
                        Recievers = role.Employees.Select(x => x.Id).ToList(),
                        Notification = null
                    });
                }

                else
                {
                    //reject delete if entity has references in any other tables
                    return await Task.FromResult(output.CreateResponse(MessageCodes.RelatedDataExist));

                }

                return output.CreateResponse(true);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<List<PageDto>>> GetLookUpPagesAsync()
        {
            var output = new Response<List<PageDto>>();
            try
            {
                var result = await _PageRepository.GetAllListAsync();
                return output.CreateResponse(_mapper.Map<List<PageDto>>(result));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        public async Task<IResponse<List<ActionDto>>> GetLookUpActionsAsync()
        {
            var output = new Response<List<ActionDto>>();
            try
            {
                var result = await _ActionRepository.GetAllListAsync();
                return output.CreateResponse(_mapper.Map<List<ActionDto>>(result));
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
        #region Helper
        private bool IsUniqueRoleName(JsonLanguageModel name, LangEnum lang = LangEnum.Default, int? roleId = null)
        {
            if (name == null)
                return false;

            return lang == LangEnum.Default
              ? _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.default") == name.Default.Trim())
                             .WhereIf(a => a.Id != roleId, roleId.HasValue).Any()
              : _roleRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                             .Where(a => Json.Value(a.Name, "$.ar") == name.Ar.Trim())
                             .WhereIf(a => a.Id != roleId, roleId.HasValue).Any();
        }
        #endregion
    }
}
