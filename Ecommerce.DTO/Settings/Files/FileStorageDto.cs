using System;

namespace Ecommerce.DTO.Settings.Files
{
    public class FileStorageDto : BaseDto
    {
        public int Id { get; set; }
        public Guid Name { get; set; }
        public string RealName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public decimal FileSize { get; set; }
        public int FileTypeId { get; set; }
        public string FileType { get; set; }
    }
    public class FileStorageBlobDto : BaseDto
    {
        public int Id { get; set; }
        public Guid Name { get; set; }
        public string RealName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public decimal FileSize { get; set; }
        public int FileTypeId { get; set; }
        public string FileType { get; set; }
    }
}
