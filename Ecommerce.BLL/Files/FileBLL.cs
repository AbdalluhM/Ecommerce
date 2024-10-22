using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Files;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Files;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.DTO.ChatMessage;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Files
{
    public class FileBLL : BaseBLL, IFileBLL
    {
        private readonly IRepository<FileStorage> _fileRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FileBLL(IRepository<FileStorage> fileRepository,
                       IMapper mapper,
                       IUnitOfWork unitOfWork)
            : base(mapper)
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IResponse<FileStorage> UploadFile(IFileDto fileDto, int? currentUserId = null)
        {
            var output =  new Response<FileStorage>();

            try
            {
                var filePathIncludingBase = Path.Combine(fileDto.FileBaseDirectory, fileDto.FilePath);
                var slashIndex = fileDto.File.ContentType.IndexOf('/');
                var contentType = fileDto.File.ContentType.Substring(0, slashIndex); 
                var newFileName = Guid.Empty;

                // get file type and validate if supported or not.
                var isSupportedType = Enum.TryParse<FileTypeEnum>(contentType, true, out var fileType);

                var extension = Path.GetExtension(fileDto.File.FileName);

                using (var memoryStream = new MemoryStream())
                {
                    fileDto.File.CopyTo(memoryStream);
                    
                    newFileName = SaveFile(memoryStream, filePathIncludingBase, extension);
                }

                // save file in dbset.
                var fileCreated = _fileRepository.Add(new FileStorage()
                {
                    Name = newFileName,
                    RealName = fileDto.File.FileName,
                    Extension = extension,
                    Path = fileDto.FilePath,
                    FullPath = Path.Combine(fileDto.FilePath, $"{newFileName}{extension}"),
                    FileSize = fileDto.File.Length,
                    FileTypeId = (int)fileType,
                    ContentType = fileDto.File.ContentType,

                    CreateDate = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                });
                return output.CreateResponse(fileCreated);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

       
        public IResponse<string> GetFilePath( int id )
        {
            var output =  new Response<string>();

            try
            {
                var file = _fileRepository.GetById(id);

                if (file is not null)
                    return output.CreateResponse(MessageCodes.NotFound);

                return output.CreateResponse(file.FullPath);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public List<MediaDto> GetAllFiles(InputFileDto input)
        {
          var res=  _fileRepository.Where(e => e.TableNameId == (int)input.TableName && e.EntityId == input.EntityId).ToList();

          return _mapper.Map<List<MediaDto>>(res);
        }
        #region Helpers.
        private Guid SaveFile( MemoryStream memoryStream, string path, string extension )
        {
            try
            {
                var fileName = Guid.NewGuid();
                // create directory if it's not exist.
                Directory.CreateDirectory(path);

                var fullName = $"{fileName}{extension}";
                var filePath = Path.Combine(path, fullName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    memoryStream.WriteTo(stream);
                }

                return fileName;
            }
            catch
            {
                throw;
            }
            
            
        }
        #endregion


    }
}
