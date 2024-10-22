using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.DTO.Files;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.DTO.ChatMessage;

namespace Ecommerce.BLL.Files
{
    public interface IFileBLL
    {
        IResponse<FileStorage> UploadFile(IFileDto fileDto, int? currentUserId = null);
        List<MediaDto> GetAllFiles(InputFileDto input);

        //ITResponse<RetrieveFileDto> PreviewFile(int id);
    }

    public interface IBlobFileBLL
    {
        Task<IResponse<FileStorage>> UploadFileAsync(SingleFilebaseDto file, int? currentUserId = null, int? entityId = null, int? nameId = null);
        Task<IResponse<FileStorage>> UpdateFileAsync(SingleFilebaseDto file, FileStorage fileStorage, int? currentUserId = null);
    }
}
