using AutoMapper;

using Ecommerce.Core.Entities;
using Ecommerce.DTO.Applications.VersionAddOns;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Applications.VersionModules;

using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.DTO.Notifications;
using Newtonsoft.Json;
using Ecommerce.Core.Enums.Notifications;
namespace Ecommerce.BLL.Mapping
{
    #region DtoToEntity

    public class CreateVersionFeatureTypeConverter : ITypeConverter<CreateVersionFeatureAPIInputDto, CreateVersionFeatureInputDto>
    {
        public CreateVersionFeatureInputDto Convert( CreateVersionFeatureAPIInputDto source, CreateVersionFeatureInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new CreateVersionFeatureInputDto() { Versions = new List<AssignFeatureVersionsDto>() };

            destination.CreateDate = DateTime.UtcNow;
            destination.FeatureId = source.FeatureId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;

            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignFeatureVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        FeatureId = source.FeatureId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        CreateDate = destination.CreateDate
                    });
                });
            }

            return destination;
        }



    }

    public class UpdateVersionFeatureTypeConverter : ITypeConverter<UpdateVersionFeatureAPIInputDto, UpdateVersionFeatureInputDto>
    {
        public UpdateVersionFeatureInputDto Convert( UpdateVersionFeatureAPIInputDto source, UpdateVersionFeatureInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new UpdateVersionFeatureInputDto() { Versions = new List<AssignFeatureVersionsDto>() };

            destination.CreateDate = DateTime.UtcNow;
            destination.FeatureId = source.FeatureId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;
            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignFeatureVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        FeatureId = source.FeatureId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        CreateDate = destination.CreateDate
                    });
                });
            }

            return destination;
        }



    }

    public class CreateVersionModuleTypeConverter : ITypeConverter<CreateVersionModuleAPIInputDto, CreateVersionModuleInputDto>
    {
        public CreateVersionModuleInputDto Convert( CreateVersionModuleAPIInputDto source, CreateVersionModuleInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new CreateVersionModuleInputDto() { Versions = new List<AssignModuleVersionsDto>() };

            destination.CreateDate = DateTime.UtcNow;
            destination.ModuleId = source.ModuleId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;

            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignModuleVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        ModuleId = source.ModuleId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        CreateDate = destination.CreateDate
                    });
                });
            }

            return destination;
        }



    }

    public class UpdateVersionModuleTypeConverter : ITypeConverter<UpdateVersionModuleAPIInputDto, UpdateVersionModuleInputDto>
    {
        public UpdateVersionModuleInputDto Convert( UpdateVersionModuleAPIInputDto source, UpdateVersionModuleInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new UpdateVersionModuleInputDto() { Versions = new List<AssignModuleVersionsDto>() };

            destination.CreateDate = DateTime.UtcNow;
            destination.ModuleId = source.ModuleId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;

            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignModuleVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        ModuleId = source.ModuleId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        CreateDate = destination.CreateDate
                    });
                });
            }

            return destination;
        }



    }


    public class CreateVersionAddOnTypeConverter : ITypeConverter<CreateVersionAddOnAPIInputDto, CreateVersionAddOnInputDto>
    {
        public CreateVersionAddOnInputDto Convert( CreateVersionAddOnAPIInputDto source, CreateVersionAddOnInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new CreateVersionAddOnInputDto() { Versions = new List<AssignAddOnVersionsDto>() };

            destination.CreateDate = DateTime.UtcNow;
            destination.AddOnId = source.AddOnId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;

            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignAddOnVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        AddOnId = source.AddOnId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        CreateDate = destination.CreateDate
                    });
                });
            }

            return destination;
        }



    }

    public class UpdateVersionAddOnTypeConverter : ITypeConverter<UpdateVersionAddOnAPIInputDto, UpdateVersionAddOnInputDto>
    {
        public UpdateVersionAddOnInputDto Convert( UpdateVersionAddOnAPIInputDto source, UpdateVersionAddOnInputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new UpdateVersionAddOnInputDto() { Versions = new List<AssignAddOnVersionsDto>() };

            destination.ModifiedDate = DateTime.UtcNow;
            destination.AddOnId = source.AddOnId;
            destination.IsActive = source.IsActive;
            destination.ApplicationId = source.ApplicationId;

            if (source.Versions != null)
            {
                source.Versions.ForEach(x =>
                {
                    destination.Versions.Add(new AssignAddOnVersionsDto
                    {
                        Id = x.Id,
                        IsActive = source.IsActive,
                        AddOnId = source.AddOnId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        //CreateDate = destination.CreateDate,
                        //ModifiedDate = destination.ModifiedDate
                    });
                });
            }

            return destination;
        }



    }
#endregion
    #region EntityToDto
    public class GetVersionFeatureOutputListToEntityTypeConverter : ITypeConverter<List<VersionFeature>, GetVersionFeatureOutputDto>
    {
        public GetVersionFeatureOutputDto Convert( List<VersionFeature> source, GetVersionFeatureOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionFeatureOutputDto { Versions = new List<GetAssignedFeatureVersionsDto>() };

            destination.ApplicationName = source.FirstOrDefault().Version.Application.Name;
            destination.FeatureName = source.FirstOrDefault().Feature.Name;

            destination.ApplicationId = source.FirstOrDefault().Version.ApplicationId;
            destination.FeatureId = source.FirstOrDefault().FeatureId;
            destination.IsActive = source.FirstOrDefault().IsActive;
            if (source != null)
            {
                source.ForEach(x =>
                {
                    destination.Versions.Add(new GetAssignedFeatureVersionsDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        FeatureId = x.FeatureId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        ApplicationName = destination.ApplicationName,
                        FeatureName = destination.FeatureName,
                        VersionName = x.Version.Name
                    });
                });
            }

            return destination;
        }



    }
    public class GetVersionFeatureOutputTypeConverter : ITypeConverter<VersionFeature, GetVersionFeatureOutputDto>
    {
        public GetVersionFeatureOutputDto Convert( VersionFeature source, GetVersionFeatureOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionFeatureOutputDto { Versions = new List<GetAssignedFeatureVersionsDto>() };

            destination.ApplicationName = source.Version.Application.Name;
            destination.FeatureName = source.Feature.Name;

            destination.ApplicationId = source.Version.ApplicationId;
            destination.FeatureId = source.FeatureId;
            destination.IsActive = source.IsActive;
            if (source != null)
            {

                destination.Versions.Add(new GetAssignedFeatureVersionsDto
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    FeatureId = source.FeatureId,
                    VersionId = source.VersionId,
                    MoreDetail = source.MoreDetail,
                    ApplicationName = destination.ApplicationName,
                    FeatureName = destination.FeatureName,
                    VersionName = source.Version.Name
                });

            }

            return destination;
        }



    }


    public class GetVersionModuleOutputListToEntityTypeConverter : ITypeConverter<List<VersionModule>, GetVersionModuleOutputDto>
    {
        public GetVersionModuleOutputDto Convert( List<VersionModule> source, GetVersionModuleOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionModuleOutputDto { Versions = new List<GetAssignedModuleVersionsDto>() };

            destination.ApplicationName = source.FirstOrDefault().Version.Application.Name;
            destination.ModuleName = source.FirstOrDefault().Module.Name;

            destination.ApplicationId = source.FirstOrDefault().Version.ApplicationId;
            destination.ModuleId = source.FirstOrDefault().ModuleId;
            destination.IsActive = source.FirstOrDefault().IsActive;
            if (source != null)
            {
                source.ForEach(x =>
                {
                    destination.Versions.Add(new GetAssignedModuleVersionsDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        ModuleId = x.ModuleId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        ApplicationName = destination.ApplicationName,
                        ModuleName = destination.ModuleName,
                        VersionName = x.Version.Name
                    });
                });
            }

            return destination;
        }



    }
    public class GetVersionModuleOutputTypeConverter : ITypeConverter<VersionModule, GetVersionModuleOutputDto>
    {
        public GetVersionModuleOutputDto Convert( VersionModule source, GetVersionModuleOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionModuleOutputDto { Versions = new List<GetAssignedModuleVersionsDto>() };

            destination.ApplicationName = source.Version.Application.Name;
            destination.ModuleName = source.Module.Name;

            destination.ApplicationId = source.Version.ApplicationId;
            destination.ModuleId = source.ModuleId;
            destination.IsActive = source.IsActive;
            if (source != null)
            {

                destination.Versions.Add(new GetAssignedModuleVersionsDto
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    ModuleId = source.ModuleId,
                    VersionId = source.VersionId,
                    MoreDetail = source.MoreDetail,
                    ApplicationName = destination.ApplicationName,
                    ModuleName = destination.ModuleName,
                    VersionName = source.Version.Name
                });

            }

            return destination;
        }



    }


    public class GetVersionAddOnOutputListToEntityTypeConverter : ITypeConverter<List<VersionAddon>, GetVersionAddOnOutputDto>
    {
        public GetVersionAddOnOutputDto Convert( List<VersionAddon> source, GetVersionAddOnOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionAddOnOutputDto { Versions = new List<GetAssignedAddOnVersionsDto>() };

            destination.ApplicationName = source.FirstOrDefault().Version.Application.Name;
            destination.AddOnName = source.FirstOrDefault().Addon.Name;

            destination.ApplicationId = source.FirstOrDefault().Version.ApplicationId;
            destination.AddOnId = source.FirstOrDefault().AddonId;
            destination.IsActive = source.FirstOrDefault().IsActive;
            if (source != null)
            {
                source.ForEach(x =>
                {
                    destination.Versions.Add(new GetAssignedAddOnVersionsDto
                    {
                        Id =x.Id,
                        IsActive = x.IsActive,
                        AddOnId = x.AddonId,
                        VersionId = x.VersionId,
                        MoreDetail = x.MoreDetail,
                        ApplicationName = destination.ApplicationName,
                        AddOnName = destination.AddOnName,
                        VersionName = x.Version.Name
                    });
                });
            }

            return destination;
        }



    }
    public class GetVersionAddOnOutputTypeConverter : ITypeConverter<VersionAddon, GetVersionAddOnOutputDto>
    {
        public GetVersionAddOnOutputDto Convert( VersionAddon source, GetVersionAddOnOutputDto destination, ResolutionContext context )
        {
            if (destination == null)
                destination = new GetVersionAddOnOutputDto { Versions = new List<GetAssignedAddOnVersionsDto>() };

            destination.ApplicationName = source.Version.Application.Name;
            destination.AddOnName = source.Addon.Name;

            destination.ApplicationId = source.Version.ApplicationId;
            destination.AddOnId = source.AddonId;
            destination.IsActive = source.IsActive;
            if (source != null)
            {

                destination.Versions.Add(new GetAssignedAddOnVersionsDto
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    AddOnId = source.AddonId,
                    VersionId = source.VersionId,
                    MoreDetail = source.MoreDetail,
                    ApplicationName = destination.ApplicationName,
                    AddOnName = destination.AddOnName,
                    VersionName = source.Version.Name
                });

            }

            return destination;
        }



    }


    public class GetNotificationOutputDtoTypeConverter : ITypeConverter<Notification, GetNotificationOutputDto>
    {
        public GetNotificationOutputDto Convert(Notification source, GetNotificationOutputDto destination, ResolutionContext context)
        {
            if (destination == null)
                destination = new GetNotificationOutputDto();
            try
            {
                destination.Id = source.Id;
                destination.CustomerId = source.CustomerId;
                destination.CreateDate = source.CreateDate;
                destination.CreatedByEmployeeId = source.CreatedByEmployeeId;
                destination.ReadByEmployeeId = source.ReadByEmployeeId;
                destination.HiddenByEmployeeId = source.HiddenByEmployeeId;
                destination.IsRead = source.IsRead;
                destination.IsHide = source.IsHide;
                destination.CustomerName = source.Customer.Name;
                destination.ActionTypeName = source.NotificationAction.NotificationActionType.Name;
                destination.CreatedByEmployeeUserName = source.NotificationAction.IsCreatedBySystem ? "System" : source.NotificationAction.IsAdminSide ? source.Customer.Name : source.CreatedByEmployee?.UserName;
                destination.ReadByEmployeeUserName = source.ReadByEmployee?.UserName;
                destination.HiddenByEmployeeUserName = source.HiddenByEmployee?.UserName;
                destination.ReadAt = source.ReadAt;
                destination.HiddenAt = source.HiddenAt;
                destination.PageDetailId = source.NotificationAction.NotificationActionPageId.Value;
                destination.TicketId = source.TicketId;
                destination.InvoiceId = source.InvoiceId;
                destination.LicenceId = source.LicenceId;
                string[] valuesForAppenddescription = new string[10];
                var jsonDescription = JsonConvert.DeserializeObject<Dictionary<string, string>>(source.NotificationAction.Description).Values;
                if (source.InvoiceId != null && source?.Invoice?.CustomerSubscription != null
                    && ((source.Invoice.CustomerSubscription.IsAddOn
                    && source.Invoice.CustomerSubscription.AddonSubscriptions != null
                    && source.Invoice.CustomerSubscription.AddonSubscriptions.Count > 0)
                    || (!source.Invoice.CustomerSubscription.IsAddOn
                    && source.Invoice.CustomerSubscription.VersionSubscriptions != null
                    && source.Invoice.CustomerSubscription.VersionSubscriptions.Count > 0)))
                {

                    destination.IsAddOn = source.Invoice.CustomerSubscription.IsAddOn;
                    destination.VersionSubscribtionId = !source.Invoice.CustomerSubscription.IsAddOn ? source.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().Id : 0;
                    destination.AddonSubscribtionId = source.Invoice.CustomerSubscription.IsAddOn ? source.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().Id : 0;
                    valuesForAppenddescription[0] = source.InvoiceId.ToString();//invoiceID
                    valuesForAppenddescription[1] = JsonConvert.DeserializeObject<Dictionary<string, string>>(!source.Invoice.CustomerSubscription.IsAddOn ? source.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName : source.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName).First().Value;//version/addon en
                    valuesForAppenddescription[2] = JsonConvert.DeserializeObject<Dictionary<string, string>>(!source.Invoice.CustomerSubscription.IsAddOn ? source.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName : source.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName).Skip(1).First().Value; //version/addon ar
                    valuesForAppenddescription[3] = source.Invoice.CustomerSubscription?.Customer?.Contract?.Serial;//serail
                }
                else if (source.LicenceId != null && source?.Licence?.CustomerSubscription != null
                    && ((source.Licence.CustomerSubscription.IsAddOn
                    && source.Licence.CustomerSubscription.AddonSubscriptions != null
                    && source.Licence.CustomerSubscription.AddonSubscriptions.Count > 0)
                    || (!source.Licence.CustomerSubscription.IsAddOn
                    && source.Licence.CustomerSubscription.VersionSubscriptions != null
                    && source.Licence.CustomerSubscription.VersionSubscriptions.Count > 0)))
                {
                    destination.IsAddOn = source.Licence.CustomerSubscription.IsAddOn;
                    destination.VersionSubscribtionId = !source.Licence.CustomerSubscription.IsAddOn ? source.Licence.CustomerSubscription.VersionSubscriptions.FirstOrDefault().Id : 0;
                    destination.AddonSubscribtionId = source.Licence.CustomerSubscription.IsAddOn ? source.Licence.CustomerSubscription.AddonSubscriptions.FirstOrDefault().Id : 0;
                    valuesForAppenddescription[1] = JsonConvert.DeserializeObject<Dictionary<string, string>>(!source.Licence.CustomerSubscription.IsAddOn ? source.Licence.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName : source.Licence.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName).First().Value;//version/addon en
                    valuesForAppenddescription[2] = JsonConvert.DeserializeObject<Dictionary<string, string>>(!source.Licence.CustomerSubscription.IsAddOn ? source.Licence.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName : source.Licence.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName).Skip(1).First().Value; //version/addon ar
                    valuesForAppenddescription[3] = source.Licence.Serial;//serail
                }
                try
                {
                    if (source.RefundRequestId != null && source.RefundRequest != null)
                    {
                        valuesForAppenddescription[4] = source.RefundRequest.Reason;//refund reason
                    }
                }
                catch (Exception exRefund)
                {

                }

                if (source.TicketRefrences != null)
                {
                    var jsonTicketsRerences = JsonConvert.DeserializeObject<Dictionary<string, string>>(source.TicketRefrences).Values.ToArray();
                    if (jsonTicketsRerences.Length >= 2)
                    {
                        valuesForAppenddescription[5] = jsonTicketsRerences[0]; //for ticket no
                        valuesForAppenddescription[6] = jsonTicketsRerences[1]; //for sendername no
                    }
                }
                //destination.Description = "{\"default\":\"" + string.Format(jsonDescription.First(), valuesForAppenddescription) + "\",\"ar\":\"" + 
               //    string.Format(jsonDescription.Skip(1).First(), valuesForAppenddescription) + "\"}".Replace("\r\n","");
                 destination.Description = 
                     JsonConvert.SerializeObject(
                 new
                 {
                     defaultEn = string.Format(jsonDescription.First(), valuesForAppenddescription),
                     ar = string.Format(jsonDescription.Skip(1).First(), valuesForAppenddescription)
                 }).Replace("defaultEn", "default");
               // destination.Description = string.Format(source.NotificationAction.Description.ToString(), valuesForAppenddescription);
            }
            catch(Exception ex)
            {

            }
            return destination;
        }
    }
    #endregion

}
