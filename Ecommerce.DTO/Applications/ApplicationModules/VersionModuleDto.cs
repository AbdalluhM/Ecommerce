using Ecommerce.DTO;
using Ecommerce.DTO.Applications.VersionModules;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.VersionModules
{
    public class VersionModuleDto : BaseDto
    {
       public int ApplicationId { get; set; }
        public int ModuleId { get; set; }
    }
    public class CreateVersionModuleAPIInputDto : VersionModuleDto
    {
        [JsonIgnore]
        public bool IsActive { get; set; } = true;

        public List<AssignAPIModuleVersionsDto> Versions { get; set; }


    }
    public class UpdateVersionModuleAPIInputDto : CreateVersionModuleAPIInputDto
    {

       


    }

    public class CreateVersionModuleInputDto : VersionModuleDto
    {

        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        public List<AssignModuleVersionsDto> Versions { get; set; }
    }


}
public class UpdateVersionModuleInputDto : CreateVersionModuleInputDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionModuleInputDto : VersionModuleDto

{

    //public int Id { get; set; }
}
public class GetAllVersionModuleInputDto : BaseDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }

}

public class DeleteApplicatinModuleInputDto: VersionModuleDto
{
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionModuleOutputDto : VersionModuleDto
{
    public int Id { get; set; }

    public string ApplicationName { get; set; }
    public string ModuleName { get; set; }
    public bool IsActive { get; set; }

    public List<GetAssignedModuleVersionsDto> Versions { get; set; }


}
public class GetUnAsignedModuleOutputDto 
{
    public int Id { get; set; }
    public string ModuleName { get; set; }
}

public class AssignAPIModuleVersionsDto : BaseDto
{
    public int Id { get; set; }
    public int VersionId { get; set; }
    public string MoreDetail { get; set; }

}

public class AssignModuleVersionsDto : AssignAPIModuleVersionsDto
{
    //public int ApplicationId { get; set; }
    public int ModuleId { get; set; }
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
public class GetAssignedModuleVersionsDto : AssignModuleVersionsDto
{
    public string ApplicationName { get; set; }
    public string ModuleName { get; set; }
    public string VersionName { get; set; }


}

