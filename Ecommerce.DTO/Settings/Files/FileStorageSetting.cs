namespace Ecommerce.DTO.Settings.Files
{

    public class FileStorageSetting
    {
        public string BaseUrl { get; set; }

        public string BaseCustomerUrl { get; set; }

        public string BlobBaseUrl { get; set; }

        public UploadedFile Files { get; set; }
    }
}
