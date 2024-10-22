using Ecommerce.DTO;
using Ecommerce.DTO.Applications.VersionAddOns;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.VersionAddOns
{
    public class VersionAddOnDto : BaseDto
    {
        public int ApplicationId { get; set; }
        public int AddOnId { get; set; }
    }
    public class CreateVersionAddOnAPIInputDto : VersionAddOnDto
    {

        [JsonIgnore]
        public bool IsActive { get; set; } = true;
        public List<AssignAPIAddOnVersionsDto> Versions { get; set; }


    }
    public class UpdateVersionAddOnAPIInputDto : CreateVersionAddOnAPIInputDto
    {




    }

    public class CreateVersionAddOnInputDto : VersionAddOnDto
    {

        public bool IsActive { get; set; } 
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        public List<AssignAddOnVersionsDto> Versions { get; set; }
    }


}
public class UpdateVersionAddOnInputDto : CreateVersionAddOnInputDto
{
     public int Id { get; set; }
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionAddOnInputDto : VersionAddOnDto

{

    //public int Id { get; set; }
}
public class GetApplicationByIdInputDto : BaseDto
{
    public int ApplicationId { get; set; }

    //public int Id { get; set; }
}

public class DeleteApplicatinAddOnInputDto : VersionAddOnDto
{
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionAddOnOutputDto : VersionAddOnDto
{
    public int Id { get; set; }
    public string ApplicationName { get; set; }
    public string AddOnName { get; set; }
    public bool IsActive { get; set; }

    public List<GetAssignedAddOnVersionsDto> Versions { get; set; }


}
public class GetUnAsignedAddOnOutputDto
{
    public int Id { get; set; }
    public string AddOnName { get; set; }
}
public class AssignAPIAddOnVersionsDto : BaseDto
{
    public int Id { get; set; }

    public int VersionId { get; set; }
    public string MoreDetail { get; set; }

}

public class AssignAddOnVersionsDto : AssignAPIAddOnVersionsDto
{
    //public int ApplicationId { get; set; }
    public int AddOnId { get; set; }
    public bool IsActive { get; set; }
    //public bool IsSelected { get; set; }
    [JsonIgnore]
    public DateTime CreateDate { get; set; }
    [JsonIgnore]
    public int CreatedBy { get; set; }
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetAssignedAddOnVersionsDto : AssignAddOnVersionsDto
{
    public string ApplicationName { get; set; }
    public string AddOnName { get; set; }
    public string VersionName { get; set; }


}

