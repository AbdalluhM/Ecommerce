using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ecommerce.BLL.Customers.CustomerSubscriptions;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.WorkSpaces;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.WorkSpaces;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.WorkSpaces;
using Ecommerce.Helper.Security;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.WorkSpaces.WorkSpaceValidator;

namespace Ecommerce.BLL.WorkSpaces
{
    public class WorkSpaceBLL : BaseBLL, IWorkSpaceBLL
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Workspace> _workSpaceRepository;
        private readonly IRepository<SimpleDatabas> _simpleDatabasRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CurrencyTable> _currencyTableRepository;
        private readonly IRepository<VersionSubscription> _versionSubscriptionRepository;
        private readonly IBlobFileBLL _fileBLL;
        private readonly ICustomerSubscriptionBLL _customerSubscriptionBLL;
        private readonly IConfiguration _configuration;
        private const string SharedKey = "jhk54*(3JOdIu79dksjh#@#(";

        public WorkSpaceBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Workspace> taxRepository, IRepository<CurrencyTable> currencyTableRepository, IRepository<SimpleDatabas> simpleDatabasRepository, IBlobFileBLL fileBLL, IRepository<Customer> customerRepository, IConfiguration configuration, ICustomerSubscriptionBLL customerSubscriptionBLL, IRepository<VersionSubscription> versionSubscriptionRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _workSpaceRepository = taxRepository;
            _currencyTableRepository = currencyTableRepository;
            _fileBLL = fileBLL;
            _customerRepository = customerRepository;
            _simpleDatabasRepository = simpleDatabasRepository;
            _configuration = configuration;
            _customerSubscriptionBLL = customerSubscriptionBLL;
            _versionSubscriptionRepository = versionSubscriptionRepository;
        }

        public async Task<IResponse<WorkspaceDetailsDto>> CreateAsync(CreateWorkSpaceDto inputDto)
        {
            var response = new Response<WorkspaceDetailsDto>();

            try
            {
                var validator = await new CreateWorkSpaceValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                if (IsAlreadyExist(inputDto.Alias, inputDto.CustomerId))
                    return response.CreateResponse(MessageCodes.AlreadyExists, inputDto.Alias);

                var customerWorkSpaces = _workSpaceRepository.Where(w => w.CustomerId == inputDto.CustomerId)
                                                                          .ToList();

                if ((inputDto.VersionSubscriptionId == null || inputDto.VersionSubscriptionId == 0) && inputDto.IsCloud && customerWorkSpaces.Any(w => w.StatusId == (int)WorkSpaceStatusEnum.Trial ||
                                                                                                  w.StatusId == (int)WorkSpaceStatusEnum.Extended))
                {
                    return response.CreateResponse(MessageCodes.AlreadyExists, nameof(WorkSpaceStatusEnum.Trial));
                }

                //Todo check supcription
                if (inputDto.VersionSubscriptionId != null)
                {
                    var versionSubscription = await _versionSubscriptionRepository.GetByIdAsync(inputDto.VersionSubscriptionId.Value);
                    if (versionSubscription == null)
                    {
                        return response.CreateResponse(MessageCodes.NotFound, nameof(VersionSubscription));
                    }
                    var subscriptionStatus = _customerSubscriptionBLL.GetSubscriptionStatus(versionSubscription.CustomerSubscriptionId);
                    if (!subscriptionStatus.IsValid)
                    {
                        return response.CreateResponse(MessageCodes.HasUnPaidInvoices, nameof(Invoice));
                    }

                    if (customerWorkSpaces.Any(w => w.StatusId == (int)WorkSpaceStatusEnum.Paid &&
                                                                                             w.VersionSubscriptionId == inputDto.VersionSubscriptionId))
                    {
                        return response.CreateResponse(MessageCodes.AlreadyExists, nameof(WorkSpaceStatusEnum.Paid));
                    }
                }



                FileStorage file = null;
                if (inputDto.File != null)
                {
                    var createdFileResult = await _fileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = inputDto.FileDto });
                    if (!createdFileResult.IsSuccess)
                        return response.AppendErrors(createdFileResult.Errors).CreateResponse();
                    file = createdFileResult.Data;
                }

                var workSpace = CreateMapAndInitializeWorkspace(inputDto, file);

                if (inputDto.IsDefault)
                {
                    workSpace.StatusId = (int)WorkSpaceStatusEnum.Trial;
                    SetDefaultCurrencyIds(workSpace, inputDto.CustomerId);
                }
                else
                    workSpace.StatusId = (int)WorkSpaceStatusEnum.Paid;


                await AddWorkspaceAsync(workSpace);



                await EncryptAndCommitWorkspaceConnectionString(workSpace);

                var detailsDto = _mapper.Map<WorkspaceDetailsDto>(workSpace);

                return response.CreateResponse(detailsDto);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }



        public async Task<IResponse<int>> UpdateAsync(UpdateWorkSpaceDto inputDto)
        {
            var response = new Response<int>();
            try
            {
                var validator = await new UpdateWorkSpaceValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var entity = await _workSpaceRepository.GetByIdAsync(inputDto.Id);
                if (entity == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(Workspace));


                if (IsAlreadyExist(inputDto.Alias, inputDto.CustomerId, entity.Id))
                    return response.CreateResponse(MessageCodes.AlreadyExists, inputDto.Alias);

                var mapped = _mapper.Map(inputDto, entity);

                if (inputDto.IsDefault)
                    SetDefaultCurrencyIds(mapped, inputDto.CustomerId);

                mapped.ConnectionString = GetConnectionString(inputDto.IsCloud, inputDto.ServerIP, inputDto.DatabaseName, inputDto.DatabaseTypeId).Item1;

                //     UpdateWorkspaceName(mapped);

                FileStorage file = null;
                if (inputDto.File != null)
                {
                    var createdFileResult = await _fileBLL.UploadFileAsync(new SingleFilebaseDto { FileDto = inputDto.FileDto });
                    if (!createdFileResult.IsSuccess)
                        return response.AppendErrors(createdFileResult.Errors).CreateResponse();
                    file = createdFileResult.Data;
                }

                mapped.Image = file ?? entity.Image;

                _unitOfWork.Commit();

                return response.CreateResponse(mapped.Id);
            }
            catch (Exception e)
            {

                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<int>> ExtendWorkSpaceAsync(int workSpaceId)
        {
            var response = new Response<int>();
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                var workSpace = await _workSpaceRepository.GetByIdAsync(workSpaceId);
                if (workSpace == null)
                    return response.CreateResponse(MessageCodes.NotFound, nameof(Workspace));


                //Todo new message for this case
                if (workSpace.StatusId != (int)WorkSpaceStatusEnum.Trial)
                    return response.CreateResponse(MessageCodes.NotAllowed, workSpace.Name);

                //Todo new message for this case
                if (workSpace.ExpirationDate >= DateTime.UtcNow)
                    return response.CreateResponse(MessageCodes.AlreadyExists, workSpace.Name);

                using (var context = new RestoreDbContext(connectionString))
                {
                    var query = $@"ALTER DATABASE {workSpace.DatabaseName} SET ONLINE;";
                    context.Database.ExecuteSqlRaw(query);
                }

                workSpace.StatusId = (int)WorkSpaceStatusEnum.Extended;
                workSpace.ModifiedDate = DateTime.UtcNow;
                workSpace.ExpirationDate = DateTime.UtcNow.AddDays(14);

                return response.CreateResponse(workSpaceId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IResponse<List<DexefCountryDto>>> GetDexefCountry()
        {
            var response = new Response<List<DexefCountryDto>>();
            try
            {
                var result = await _currencyTableRepository.GetAllListAsync();

                return response.CreateResponse(_mapper.Map<List<DexefCountryDto>>(result.OrderBy(e => e.EnNameCountry).ThenBy(e => e.ArNameCountry)));
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<List<DexefCurrencyDto>>> GetDexefCurrency()
        {
            var response = new Response<List<DexefCurrencyDto>>();
            try
            {
                var result = await _currencyTableRepository.GetAllListAsync();
                return response.CreateResponse(_mapper.Map<List<DexefCurrencyDto>>(result.OrderBy(e => e.EnName).ThenBy(e => e.ArName)));
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<IEnumerable<WorkspaceDto>>> GetWorkSpacesAsync(int customerId)
        {
            var response = new Response<IEnumerable<WorkspaceDto>>();

            try
            {
                var workSpaces = await _workSpaceRepository.GetManyAsync(w => w.CustomerId == customerId);

                var workSpacesDto = _mapper.Map<IEnumerable<WorkspaceDto>>(workSpaces);

                //foreach (var workspace in workSpacesDto)
                //{
                //    workspace.ConnectionString = await AesEncryption.EncryptStringAsync(SharedKey, workspace.ConnectionString);
                //}

                return response.CreateResponse(workSpacesDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<WorkspaceDetailsDto>> GetWorkspaceDetailsAsync(int id)
        {
            var response = new Response<WorkspaceDetailsDto>();

            try
            {
                var workspace = await _workSpaceRepository.GetAsync(w => w.Id == id);

                var workspaceDto = _mapper.Map<WorkspaceDetailsDto>(workspace);

                //workspaceDto.ConnectionString = await AesEncryption.EncryptStringAsync(SharedKey, workspace.ConnectionString);

                return response.CreateResponse(workspaceDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        public async Task<IResponse<List<SimpleDatabaseDto>>> GetSimpleDatabaseAsync()
        {
            var response = new Response<List<SimpleDatabaseDto>>();

            try
            {
                var result = _simpleDatabasRepository.GetAll().ToList();

                return response.CreateResponse(_mapper.Map<List<SimpleDatabaseDto>>(result));

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }

        }
        public async Task<IResponse<int>> CreateSimpleDatabaseAsync(SimpleDatabaseDto inputDto)
        {
            var response = new Response<int>();

            try
            {
                var validator = await new SimpleDatabaseValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                inputDto.Id = 0;
                var result = await _simpleDatabasRepository.AddAsync(_mapper.Map<SimpleDatabas>(inputDto));
                await _unitOfWork.CommitAsync();
                return response.CreateResponse(result.Id);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }

        }

        public async Task<IResponse<SimpleDatabaseDto>> GetSimpleDatabaseByIdAsync(int id)
        {
            var response = new Response<SimpleDatabaseDto>();

            try
            {
                var result = await _simpleDatabasRepository.GetByIdAsync(id);

                return response.CreateResponse(_mapper.Map<SimpleDatabaseDto>(result));

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }

        }
        public async Task<IResponse<bool>> DeleteSimpleDatabase(int id)
        {
            var response = new Response<bool>();

            try
            {
                var simple = await _simpleDatabasRepository.GetByIdAsync(id);
                if (simple is null)
                    return response.CreateResponse(MessageCodes.NotFound, id.ToString());

                _simpleDatabasRepository.Delete(simple);
                await _unitOfWork.CommitAsync();

                return response.CreateResponse(true);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }

        }
        public async Task<IResponse<int>> UpdateSimpleDatabase(SimpleDatabaseDto inputDto)
        {
            var response = new Response<int>();

            try
            {
                var validator = await new SimpleDatabaseValidator().ValidateAsync(inputDto);
                if (!validator.IsValid)
                    return response.CreateResponse(validator.Errors);

                var simpleId = await _simpleDatabasRepository.GetByIdAsync(inputDto.Id);
                if (simpleId is null)
                    return response.CreateResponse(MessageCodes.NotFound, inputDto.Id.ToString());


                var result = _simpleDatabasRepository.Update(_mapper.Map<SimpleDatabas>(inputDto));
                await _unitOfWork.CommitAsync();
                return response.CreateResponse(result.Id);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }

        }
        #region Helper
        private Workspace CreateMapAndInitializeWorkspace(CreateWorkSpaceDto inputDto, FileStorage file)
        {
            var workSpace = _mapper.Map<Workspace>(inputDto);
            workSpace.Image = file;
            var (connectionString, databaseName) = GetConnectionString(inputDto.IsCloud, inputDto.ServerIP, inputDto.DatabaseName, inputDto.DatabaseTypeId);
            workSpace.ConnectionString = connectionString;
            workSpace.DatabaseName = databaseName;
            if (inputDto.IsCloud) workSpace.ServerIp = General.DexefServerIp;
            // workSpace.StatusId = (int)WorkSpaceStatusEnum.Trial;
            return workSpace;
        }

        private void SetDefaultCurrencyIds(Workspace workSpace, int customerId)
        {
            int customerCurrencyId = GetCustomerCountryCurrencyId(customerId);
            if (customerCurrencyId > 0)
            {
                workSpace.DexefCountryId = customerCurrencyId;
                workSpace.DexefCurrencyId = customerCurrencyId;
            }
        }

        private async Task AddWorkspaceAsync(Workspace workSpace)
        {
            await _workSpaceRepository.AddAsync(workSpace);
            if (!((workSpace.VersionSubscriptionId == null || workSpace.VersionSubscriptionId == 0) &&
                _workSpaceRepository.Any(w => w.CustomerId == workSpace.CustomerId && w.StatusId == (int)WorkSpaceStatusEnum.Trial ||
                                                                                  w.StatusId == (int)WorkSpaceStatusEnum.Extended)))
            {
                _unitOfWork.Commit(); // Commit at the end of the batch
            }
            if (!(workSpace.VersionSubscriptionId != null && _workSpaceRepository.Any(w => w.CustomerId == workSpace.CustomerId && w.StatusId == (int)WorkSpaceStatusEnum.Paid &&
                                                                                             w.VersionSubscriptionId == workSpace.VersionSubscriptionId)))
            {
                _unitOfWork.Commit(); // Commit at the end of the batch
            }

        }

        private void UpdateWorkspaceName(Workspace workSpace)
        {
            workSpace.Name = $"{workSpace.Name}-{workSpace.Id}-{workSpace.CustomerId}";
        }

        private async Task EncryptAndCommitWorkspaceConnectionString(Workspace workSpace)
        {
            workSpace.ConnectionString = await AesEncryption.EncryptStringAsync(SharedKey, workSpace.ConnectionString);
            _unitOfWork.Commit();
        }
        private (string connectionString, string databaseName) GetConnectionString(bool isCloud, string serverIp, string databaseName, int databaseTypeId)
        {

            if (isCloud)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                StringBuilder stringBuilder = new();
                var database = $"DEXEFDB_{stringBuilder.GetRandomString()}";
                var databaseFile = $"DEXEFDB_{stringBuilder.GetRandomString()}_Files";
                RestoreDatabase(connectionString, General.BackupDbPath, database, databaseTypeId);
                RestoreDatabase(connectionString, General.BackupDbFilePath, databaseFile, databaseTypeId, true);
                return (connectionString.Replace("master", $"{database}"), database);

            }
            else
            {

                return ($"Data source={serverIp};initial catalog={databaseName};Integrated security=False; User Id=sa;Password=~!@Dexef321;MultipleActiveResultSets=True;Connection Timeout=6000;TrustServerCertificate=True;", databaseName);
            }


        }
        public void RestoreDatabase(string connectionString, string backupPath, string databaseName, int databaseTypeId, bool isFile = false)
        {
            using (var context = new RestoreDbContext(connectionString))
            {
                var backupCommand = string.Empty;
                string moved = "Free";

                if (databaseTypeId == 0)
                {
                    if (!isFile)
                    {

                        backupCommand = RestoreQuery(databaseName, backupPath, moved);
                    }
                    else
                    {
                        moved = $"{moved}_Files";
                        backupCommand = RestoreQuery(databaseName, backupPath, moved, isFile: isFile);
                    }
                }
                else
                {

                    var database = _simpleDatabasRepository.GetById(databaseTypeId);

                    if (!isFile)
                    {
                        //Todo change database.Url to database.Path
                        backupCommand = RestoreQuery(databaseName, $"{database.FilePath}\\SWF.mtm", database.DataBaseName);
                    }
                    else
                    {
                        string movedFile = $"{database.DataBaseName}_Files";
                        //Todo change database.Url to database.Path
                        backupCommand = RestoreQuery(databaseName, $"{database.FilePath}\\SWF_Files.mtm", movedFile, isFile);
                    }
                }


                context.Database.ExecuteSqlRaw(backupCommand);


            }
        }
        private string RestoreQuery(string databaseName, string backupFilePath, string moveFile, bool isFile = false)
        {


            return $@"use master
                                  RESTORE DATABASE {databaseName}
                                  FROM DISK = N'{backupFilePath}'
                                  WITH
                                  MOVE '{moveFile}' TO 'C:\Dexef Accounting\{databaseName}.mdf',
                                  MOVE '{moveFile}_log' TO 'C:\Dexef Accounting\{databaseName}.ldf'";

        }
        public int GetCustomerCountryCurrencyId(int customerId)
        {
            var customerCountyCode = $"+{_customerRepository.GetById(customerId).Country.PhoneCode}";
            var customerCurrency = _currencyTableRepository.Where(x => x.DiallingCode.Equals(customerCountyCode)).FirstOrDefault();
            return customerCurrency.Id;
        }



        private bool IsAlreadyExist(string name, int customerId, int? excludedId = null)
        {
            return _workSpaceRepository.Where(x => x.Alias.Equals(name) && x.CustomerId == customerId)
                                       .WhereIf(x => x.Id != excludedId, excludedId.HasValue && excludedId > 0)
                                       .Any();
        }
        #endregion
    }
}
