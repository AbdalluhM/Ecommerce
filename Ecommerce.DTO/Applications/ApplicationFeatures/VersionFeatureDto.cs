using Ecommerce.DTO;
using Ecommerce.DTO.Applications.VersionFeatures;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.VersionFeatures
{
    public class VersionFeatureDto : BaseDto
    {
        public int ApplicationId { get; set; }
        public int FeatureId { get; set; }
        [JsonIgnore]
        public bool IsActive { get; set; } = true;
        public List<AssignAPIFeatureVersionsDto> Versions { get; set; }

    }
    public class CreateVersionFeatureAPIInputDto : VersionFeatureDto
    {




    }
    public class UpdateVersionFeatureAPIInputDto : CreateVersionFeatureAPIInputDto
    {
        public int Id { get; set; }



    }

    public class CreateVersionFeatureInputDto : VersionFeatureDto
    {

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        public List<AssignFeatureVersionsDto> Versions { get; set; }
    }


}
public class UpdateVersionFeatureInputDto : CreateVersionFeatureInputDto
{
    public int Id { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionFeatureInputDto : VersionFeatureDto

{
}
public class GetAllVersionFeatureInputDto : BaseDto
{
    public int ApplicationId { get; set; }

}

public class DeleteApplicatinFeatureInputDto : VersionFeatureDto
{
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetVersionFeatureOutputDto : VersionFeatureDto
{
    public int Id { get; set; }
    public string ApplicationName { get; set; }
    public string FeatureName { get; set; }
    public List<GetAssignedFeatureVersionsDto> Versions { get; set; }


}
public class GetAllVersionFeatureOutputDto
{
    public int Id { get; set; }
    public string ApplicationName { get; set; }
    public string FeatureName { get; set; }
    public List<GetVersionAssignFeatureOutputDto> Versions { get; set; }


}
public class VersionFeaturesDto
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public string Name { get; set; }
    public string FetureName { get; set; }
    public string MoreDetail { get; set; }
}
public class GetUnAsignedFeatureOutputDto
{
    public int Id { get; set; }
    public string FeatureName { get; set; }

}
public class AssignAPIFeatureVersionsDto : BaseDto
{
    public int Id { get; set; }
    public int VersionId { get; set; }
    public string MoreDetail { get; set; }

}

public class AssignFeatureVersionsDto : AssignAPIFeatureVersionsDto
{

    public int FeatureId { get; set; }
    public bool IsActive { get; set; }
    [JsonIgnore]
    public DateTime CreateDate { get; set; }
    [JsonIgnore]
    public int CreatedBy { get; set; }
    [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
}
public class GetAssignedFeatureVersionsDto : AssignFeatureVersionsDto
{
    public string ApplicationName { get; set; }
    public string FeatureName { get; set; }
    public string VersionName { get; set; }
}

public class GetVersionAssignFeatureOutputDto
{
    public int Id { get; set; }
    public string VersionName { get; set; }
}




