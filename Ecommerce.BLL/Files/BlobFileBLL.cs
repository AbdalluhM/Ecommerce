using AutoMapper;
using Dexef.Storage;
using Dexef.Storage.Azure.Credentials;
using Dexef.Storage.Enums;
using Dexef.Storage.Providers.Azure.Blobs;
using Microsoft.Extensions.Configuration;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Files;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Files;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Files;
using Ecommerce.Repositroy.Base;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Files
{
    public class BlobFileBLL : BaseBLL, IBlobFileBLL
    {
        private readonly IDexefStorageManager _dexefStorageManager;
        private readonly IAzureBlobStorageManager _azureBlobStorageManager;
        private readonly IRepository<FileStorage> _fileRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BlobFileBLL(IDexefStorageManager dexefStorageManager,
                           IRepository<FileStorage> fileRepository,
                           IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IConfiguration configuration)
            : base(mapper)
        {
            _dexefStorageManager = dexefStorageManager;

            var connection = configuration.GetConnectionString("EcommerceStorageConnection");

            _azureBlobStorageManager = _dexefStorageManager.GetProvider(ProvidersEnum.Azure).GetStorageManager(new AzureBlobCredential { ConnectionString = connection }, ManagersEnum.AzureBlob) as IAzureBlobStorageManager;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResponse<FileStorage>> UploadFileAsync(SingleFilebaseDto file, int? currentUserId = null, int? entityId = null, int? nameId = null)
        {
            var response = new Response<FileStorage>();

            try
            {


                var verifuInputDto = await new SingleFileBaseDtoValidator(20, typeof(FileTypeEnum)).ValidateAsync(file);

                if (!verifuInputDto.IsValid)
                    return response.CreateResponse(verifuInputDto.Errors);

                var filePathIncludingBase = Path.Combine(file.FileDto.FileBaseDirectory, file.FileDto.FilePath);
                //var slashIndex = file.FileDto.File.ContentType.IndexOf('/');
                //var contentType = file.FileDto.File.ContentType.Substring(0, slashIndex);
                var contentType = file.FileDto.File.ContentType.Split('/')[0];


                var newFileName = Guid.NewGuid();

                // get file type and validate if supported or not.
                var isSupportedType = Enum.TryParse<FileTypeEnum>(contentType, true, out var fileType);

                var extension = Path.GetExtension(file.FileDto.File.FileName) == string.Empty || Path.GetExtension(file.FileDto.File.FileName) == null ?
                   $".{file.FileDto.File.ContentType.Split('/')[1]}" : Path.GetExtension(file.FileDto.File.FileName);

                var BlobName = $"{file.FileDto.FilePath}/{newFileName}{extension}";

                var uploadInfo = await _azureBlobStorageManager.UploadBlobAsync(file.FileDto.ContainerName, BlobName, file.FileDto.File, file.FileDto.File.FileName);

                // save file in dbset.
                FileStorage fileCreated = CreateFileStorage(file, currentUserId, newFileName, fileType, extension, entityId, nameId);

                _unitOfWork.Commit();

                return response.CreateResponse(fileCreated);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }



        public async Task<IResponse<FileStorage>> UpdateFileAsync(SingleFilebaseDto file, FileStorage fileStorage, int? currentUserId = null)
        {
            var output = new Response<FileStorage>();
            try
            {


                //    //basic form validation
                var verifuInputDto = await new SingleFileBaseDtoValidator(2, typeof(FileTypeEnum)).ValidateAsync(file);
                if (!verifuInputDto.IsValid)
                    return output.CreateResponse(verifuInputDto.Errors);
                var fileItem = file.FileDto.File;
                var extension = Path.GetExtension(fileItem?.FileName);
                var BlobName = fileStorage.Path;
                var slashIndex = file.FileDto.File.ContentType.IndexOf('/');
                var contentType = file.FileDto.File.ContentType.Substring(0, slashIndex);
                if (fileStorage.Extension != extension)
                {
                    await _azureBlobStorageManager.DeleteBlobAsync(file.FileDto.ContainerName, BlobName);
                    var newFileName = Guid.NewGuid();
                    BlobName = file.FileDto.FilePath + "/" + newFileName.ToString() + extension;
                    fileStorage.Path = BlobName;
                }
                await _azureBlobStorageManager.UploadBlobAsync(file.FileDto.ContainerName, BlobName, fileItem, fileItem.FileName);

                Enum.TryParse<FileTypeEnum>(contentType, true, out var fileType);

                fileStorage.Extension = extension;
                fileStorage.FileTypeId = (int)fileType;
                fileStorage.FileSize = file.FileDto.File.Length;
                fileStorage.ContentType = contentType;


                var result = _fileRepository.Update(fileStorage);
                _unitOfWork.Commit();
                //map the inserted file row into output model
                //var fileMappedOutput = _mapper.Map<List<FileOutputDto>>(result);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }



        #region Helper
        private FileStorage CreateFileStorage(SingleFilebaseDto file, int? currentUserId, Guid newFileName, FileTypeEnum fileType, string extension, int? entityId = null, int? nameId = null)
        {
            return _fileRepository.Add(new FileStorage()
            {
                Name = newFileName,
                RealName = file.FileDto.File.FileName,
                Extension = extension,
                Path = file.FileDto.FilePath,
                FullPath = Path.Combine(file.FileDto.ContainerName, file.FileDto.FilePath, $"{newFileName}{extension}"),
                FileSize = file.FileDto.File.Length,
                FileTypeId = (int)fileType,
                ContentType = file.FileDto.File.ContentType,
                CreatedBy = currentUserId,
                CreateDate = DateTime.UtcNow,
                EntityId = entityId,
                TableNameId = nameId,
            });
        }




        #endregion
    }
}
