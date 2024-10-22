using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Applications.ApplicationVersions
{
    public class VersionReleaseDto
    {
        public int VersionId { get; set; }
        public string ReleaseNumber { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public class VersionReleaseOutputDto : VersionReleaseDto
    {
        public int Id { get; set; }
    }
}
