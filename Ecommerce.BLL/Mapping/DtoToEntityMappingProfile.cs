using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.License;
using Ecommerce.Core.Enums.Reviews;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonSliders.Inputs;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationSlider;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Applications.VersionAddOns;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Applications.VersionModules;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Contracts.Licenses.Inputs;
using Ecommerce.DTO.Customers;
using Ecommerce.DTO.Customers.Auth.Inputs;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Customers.CustomerProduct;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Inputs;
using Ecommerce.DTO.Modules;
using Ecommerce.DTO.Modules.ModuleSlider;
using Ecommerce.DTO.Modules.ModuleTags;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Tags;
using Ecommerce.DTO.Taxes;
using Ecommerce.DTO.WorkSpaces;
using Newtonsoft.Json;
using System;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;
using static Ecommerce.DTO.Customers.HomePage.HomePageDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;
using static Ecommerce.DTO.Roles.RoleDto;
using Action = Ecommerce.Core.Entities.Action;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Mapping
{
    public class DtoToEntityMappingProfile : Profile
    {
        public DtoToEntityMappingProfile()
        {


            #region Employees
            //CreateMap<APICreateEmployeeInputDto, CreateEmployeeInputDto>()
            //  .ForMember(pl => pl.File, opt => opt.Ignore())
            //  .ForAllMembers(opts => opts.Condition(( src, dest, srcMember ) => srcMember != null));

            CreateMap<APIUpdateEmployeeInputDto, UpdateEmployeeInputDto>()
             .ForMember(pl => pl.File, opt => opt.Ignore())
             .ForMember(pl => pl.IsActive, opt => opt.Ignore())
              .ForMember(pl => pl.UserName, opt => opt.Ignore())
             .ForMember(pl => pl.Password, opt => opt.Ignore())
             .ForMember(pl => pl.EmployeeCountries, opt => opt.Ignore())
             .ForMember(pl => pl.EmployeeCountriesList, opt => opt.Ignore())
             //.ForMember(pl => pl.RoleId, opt => opt.Ignore())
             .ForMember(pl => pl.Email, opt => opt.Ignore())
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateEmployeeInputDto, Employee>()
             .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
             .ForMember(d => d.Mobile, m => m.Ignore())
            .ForMember(d => d.EmployeeCountries, m => m.MapFrom(s => s.EmployeeCountries))
                  .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //.ForMember(d => d.EmployeeCountries, m => m.MapFrom(s => s.EmployeeCountries)); 

            //CreateMap<int,UpdateAssignEmployeeToCountryInputDto>().ForMember(d => d.CountryCurrencyId, m => m.MapFrom(s => s));
            CreateMap<int, EmployeeCountry>().ForMember(d => d.CountryCurrencyId, m => m.MapFrom(s => s));


            CreateMap<UpdateEmployeeInputDto, Employee>()
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
        .ForMember(pl => pl.IsActive, opt => opt.Ignore())
        .ForMember(pl => pl.Password, opt => opt.Ignore())
        .ForMember(pl => pl.EmployeeCountries, opt => opt.Ignore())
        //.ForMember(pl => pl.RoleId, opt => opt.Ignore())
        .ForMember(pl => pl.Email, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opts => { opts.MapFrom(aa => DateTime.UtcNow); opts.Condition(dto => dto.Id != 0); })
            .ForMember(d => d.EmployeeCountries, m => m.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<AssignEmployeeToCountryInputDto, EmployeeCountry>();
            CreateMap<UpdateAssignEmployeeToCountryInputDto, EmployeeCountry>()
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            #endregion

            #region Countries
            CreateMap<CreateCountryInputDto, Country>();
            CreateMap<UpdateCountryInputDto, Country>();

            CreateMap<AssignCurrencyToCountryInputDto, CountryCurrency>()
             .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateAssignCurrencyToCountryInputDto, CountryCurrency>()
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedDate, opts => { opts.MapFrom(aa => DateTime.UtcNow); opts.Condition(dto => dto.Id != 0); })
            //.ForMember(pl => pl.CountryId, opts =>
            //            {
            //                opts.Ignore();
            //                opts.Condition(dto => dto.CountryId == 0);
            //            })
            //.ForMember(pl => pl.CurrencyId, opts =>
            //   {
            //       opts.Ignore();
            //       opts.Condition(dto => dto.CurrencyId == 0);
            //   })
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            #endregion

            #region Tags
            CreateMap<CreateTagInputDto, Tag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateTagInputDto, Tag>()
          .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
          .ForMember(pl => pl.CreateDate, options => options.Ignore())
          .ForMember(dest => dest.ModifiedDate, opts => { opts.MapFrom(aa => DateTime.UtcNow); opts.Condition(dto => dto.Id != 0); })
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<GetTagOutputDto, Tag>();



            //CreateMap<AssignEmployeeToCountryInputDto, EmployeeCountry>();
            //CreateMap<UpdateAssignEmployeeToCountryInputDto, EmployeeCountry>();
            #endregion

            #region AddonTags

            CreateMap<AddonTagDto, AddOnTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<CreateAddonTagInputDto, AddOnTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<UpdateAddonTagDto, AddOnTag>()
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(pl => pl.CreateDate, options => options.Ignore())
            .ForMember(pl => pl.AddOnId, opts =>
                    {
                        opts.Ignore();
                        opts.Condition(dto => dto.AddonId == 0);
                    })
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            #endregion

            #region AddonLabels

            CreateMap<CreateAddonLabelInputDto, AddOnLabel>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateAddonLabelInputDto, AddOnLabel>()
            .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(pl => pl.CreateDate, options => options.Ignore())
            .ForMember(pl => pl.AddOnId, opts =>
            {
                opts.Ignore();
                opts.Condition(dto => dto.AddOnId == 0);
            })
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region AddOnPrices

            CreateMap<CreateAddOnPriceInputDto, AddOnPrice>()
                .ForMember(dest => dest.CreateDate
               , opts => opts.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateAddOnPriceInputDto, AddOnPrice>()
                .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
                .ForMember(pl => pl.CreateDate, options => options.Ignore())
                .ForMember(pl => pl.AddOnId, opts =>
                {
                    opts.Ignore();
                    opts.Condition(dto => dto.AddOnId == 0);
                })
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            #endregion

            #region Price Levels
            CreateMap<NewPriceLevelDto, PriceLevel>()
                .ForMember(pl => pl.CreateDate, cd => cd.MapFrom(pld => DateTime.UtcNow));

            CreateMap<UpdatePriceLevelDto, PriceLevel>()
                .ForMember(pl => pl.CreateDate, options => options.Ignore())
                .ForMember(pl => pl.CreatedBy, options => options.Ignore())
                .ForMember(pl => pl.ModifiedDate, options => options.MapFrom(pld => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion


            #region Features

            CreateMap<CreateFeatureInputDto, Feature>()
                .ForMember(dest => dest.CreateDate
               , opts => opts.MapFrom(s => DateTime.UtcNow));
            CreateMap<UpdateFeatureInputDto, Feature>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(pl => pl.CreateDate, options => options.Ignore())
              .ForMember(pl => pl.LogoId, opts =>
              {
                  opts.Ignore();
                  opts.Condition(dto => dto.LogoId == 0);
              })
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))

            ;


            CreateMap<DeleteFeatureInputDto, Feature>()
           .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
           .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
           .ForMember(pl => pl.CreateDate, options => options.Ignore());

            CreateMap<CreateFeatureAPIInputDto, CreateFeatureInputDto>()
            .ForMember(dest => dest.CreateDate
            , opts => opts.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.File
            , opts => opts.Ignore());
            //.ForMember(dest => dest.File,
            //opts => opts.MapFrom(s => new FileDto
            //{
            //    File = s.File,
            //    FileBaseDirectory = AppContext.BaseDirectory//,
            //    //FilePath = _fileSetting.Files.Admin.Feature.Path
            //}));

            CreateMap<UpdateFeatureAPIInputDto, UpdateFeatureInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.File
               , opts => opts.Ignore());


            //.ForMember(dest => dest.File,
            //opts => opts.MapFrom(s => new FileDto
            //{
            //    File = s.File,
            //    FileBaseDirectory = AppContext.BaseDirectory//,
            //   //FilePath = _fileSetting.Files.Admin.Feature.Path
            // }));
            #endregion

            #region AddOn

            CreateMap<NewAddonDto, AddOn>()
                .ForMember(pl => pl.CreateDate, cd => cd.MapFrom(pld => DateTime.UtcNow));

            CreateMap<CreateAddOnAPIInputDto, CreateAddOnInputDto>()
         .ForMember(dest => dest.CreateDate
         , opts => opts.MapFrom(s => DateTime.UtcNow))
             .ForMember(dest => dest.File
         , opts => opts.Ignore());
            CreateMap<UpdateAddOnAPIInputDto, UpdateAddOnInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
               .ForMember(dest => dest.File
           , opts => opts.Ignore());

            CreateMap<CreateAddOnInputDto, AddOn>()
              .ForMember(dest => dest.CreateDate
             , opts => opts.MapFrom(s => DateTime.UtcNow));
            CreateMap<UpdateAddOnInputDto, AddOn>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(pl => pl.CreateDate, options => options.Ignore())
               .ForMember(pl => pl.LogoId, opts =>
               {
                   opts.Ignore();
                   opts.Condition(dto => dto.LogoId == 0);
               })
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<DeleteAddOnInputDto, AddOn>()
           .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
           .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
           .ForMember(pl => pl.CreateDate, opts => opts.Ignore());
            #endregion

            #region Addon Sliders.

            CreateMap<NewAddonSliderDto, AddOnSlider>()
                .ForMember(pl => pl.CreateDate, cd => cd.MapFrom(pld => DateTime.UtcNow))
                .ForMember(pl => pl.IsActive, cd => cd.MapFrom(pld => true));
            #endregion

            #region Module 
            CreateMap<CreateModuleAPIInputDto, CreateModuleInputDto>()
         .ForMember(dest => dest.CreateDate
         , opts => opts.MapFrom(s => DateTime.UtcNow))
             .ForMember(dest => dest.File
         , opts => opts.Ignore());
            CreateMap<UpdateModuleAPIInputDto, UpdateModuleInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
               .ForMember(dest => dest.File
           , opts => opts.Ignore());

            CreateMap<CreateModuleInputDto, Module>()
              .ForMember(dest => dest.CreateDate
             , opts => opts.MapFrom(s => DateTime.UtcNow));
            CreateMap<UpdateModuleInputDto, Module>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(x => x.CreateDate, options => options.Ignore())
               .ForMember(x => x.ImageId, opts =>
               {
                   opts.Ignore();
                   opts.Condition(dto => dto.ImageId == 0);
               })
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<DeleteModuleInputDto, Module>()
           .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
           .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
           .ForMember(x => x.CreateDate, opts => opts.Ignore());
            #endregion

            #region Module Tags

            CreateMap<ApplicationTagDto, ModuleTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<CreateModuleTagInputDto, ModuleTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<UpdateModuleTagInputDto, ModuleTag>()
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(pl => pl.CreateDate, options => options.Ignore())
            .ForMember(pl => pl.ModuleId, opts =>
            {
                opts.Ignore();
                opts.Condition(dto => dto.ModuleId == 0);
            })
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            #endregion

            #region Module Sliders
            CreateMap<CreateModuleSliderAPIInputDto, CreateModuleSliderInputDto>()
      .ForMember(dest => dest.CreateDate
      , opts => opts.MapFrom(s => DateTime.UtcNow))
          .ForMember(dest => dest.File
      , opts => opts.Ignore());
            CreateMap<UpdateModuleSliderAPIInputDto, UpdateModuleSliderInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
               .ForMember(dest => dest.File
           , opts => opts.Ignore());

            CreateMap<CreateModuleSliderInputDto, ModuleSlider>()
              .ForMember(dest => dest.CreateDate
             , opts => opts.MapFrom(s => DateTime.UtcNow));
            CreateMap<UpdateModuleSliderInputDto, ModuleSlider>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(x => x.CreateDate, options => options.Ignore())
               .ForMember(x => x.MediaId, opts =>
               {
                   opts.Ignore();
                   opts.Condition(dto => dto.MediaId == 0);
               })
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region Application 
            CreateMap<CreateApplicationAPIInputDto, CreateApplicationInputDto>()
         .ForMember(dest => dest.CreateDate
         , opts => opts.MapFrom(s => DateTime.UtcNow))
             .ForMember(dest => dest.File
         , opts => opts.Ignore());
            CreateMap<UpdateApplicationAPIInputDto, UpdateApplicationInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
           .ForMember(dest => dest.DeviceTypeId
               , opts => opts.MapFrom(s => s.DeviceTypeId == 0 ? null : s.DeviceTypeId))

               .ForMember(dest => dest.File
           , opts => opts.Ignore());

            CreateMap<CreateApplicationInputDto, Application>()
              .ForMember(dest => dest.CreateDate
             , opts => opts.MapFrom(s => DateTime.UtcNow))
              ;
            CreateMap<UpdateApplicationInputDto, Application>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(x => x.CreateDate, options => options.Ignore())
               .ForMember(x => x.ImageId, opts =>
               {
                   opts.Ignore();
                   opts.Condition(dto => dto.ImageId == 0);
               })
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<DeleteApplicationInputDto, Application>()
           .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
           .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
           .ForMember(x => x.CreateDate, opts => opts.Ignore());
            #endregion]

            #region Application Labels

            CreateMap<CreateApplicationLabelInputDto, ApplicationLabel>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateApplicationLabelInputDto, ApplicationLabel>()
            .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(pl => pl.CreateDate, options => options.Ignore())
            .ForMember(pl => pl.ApplicationId, opts =>
            {
                opts.Ignore();
                opts.Condition(dto => dto.ApplicationId == 0);
            })
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region Application Tags

            CreateMap<ApplicationTagDto, ApplicationTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<CreateApplicationTagInputDto, ApplicationTag>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<UpdateApplicationTagInputDto, ApplicationTag>()
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(pl => pl.CreateDate, options => options.Ignore())
            .ForMember(pl => pl.ApplicationId, opts =>
            {
                opts.Ignore();
                opts.Condition(dto => dto.ApplicationId == 0);
            })
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            #endregion

            #region Application Sliders
            CreateMap<CreateApplicationSliderAPIInputDto, CreateApplicationSliderInputDto>()
      .ForMember(dest => dest.CreateDate
      , opts => opts.MapFrom(s => DateTime.UtcNow))
          .ForMember(dest => dest.File
      , opts => opts.Ignore());
            CreateMap<UpdateApplicationSliderAPIInputDto, UpdateApplicationSliderInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
               .ForMember(dest => dest.File
           , opts => opts.Ignore());

            CreateMap<CreateApplicationSliderInputDto, ApplicationSlider>()
              .ForMember(dest => dest.CreateDate
             , opts => opts.MapFrom(s => DateTime.UtcNow));
            CreateMap<UpdateApplicationSliderInputDto, ApplicationSlider>()
              .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
              .ForMember(x => x.CreateDate, options => options.Ignore())
               .ForMember(x => x.MediaId, opts =>
               {
                   opts.Ignore();
                   opts.Condition(dto => dto.MediaId == 0);
               })
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region Application Versions
            CreateMap<CreateVersionAPIInputDto, CreateVersionInputDto>()
                .ForMember(dest => dest.CreateDate
                , opts => opts.MapFrom(s => DateTime.UtcNow))
                    .ForMember(dest => dest.File
                , opts => opts.Ignore());
            CreateMap<UpdateVersionAPIInputDto, UpdateVersionInputDto>()
           .ForMember(dest => dest.CreateDate
           , opts => opts.MapFrom(s => DateTime.UtcNow))
               .ForMember(dest => dest.File
       , opts => opts.Ignore());

            CreateMap<CreateVersionInputDto, Version>()
                  .ForMember(dest => dest.CreateDate
                 , opts => opts.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateVersionInputDto, Version>()
                   .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
                   .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
                   .ForMember(x => x.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
                    .ForMember(x => x.ImageId, opts =>
                    {
                        opts.Ignore();
                        opts.Condition(dto => dto.ImageId == 0);
                    })
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            #endregion

            #region Application Version Prices

            CreateMap<CreateVersionPriceInputDto, VersionPrice>()
                .ForMember(dest => dest.CreateDate
               , opts => opts.MapFrom(s => DateTime.UtcNow));
            ;
            CreateMap<UpdateVersionPriceInputDto, VersionPrice>()
                .ForMember(d => d.ModifiedDate, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
                .ForMember(pl => pl.CreateDate, options => options.Ignore())
                .ForMember(pl => pl.VersionId, opts =>
                {
                    opts.Ignore();
                    opts.Condition(dto => dto.VersionId == 0);
                })
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            #endregion

            #region Application Features

            CreateMap<CreateVersionFeatureAPIInputDto, CreateVersionFeatureInputDto>()
                .ConvertUsing<CreateVersionFeatureTypeConverter>();
            CreateMap<UpdateVersionFeatureAPIInputDto, UpdateVersionFeatureInputDto>()
              .ConvertUsing<UpdateVersionFeatureTypeConverter>();
            CreateMap<AssignFeatureVersionsDto, VersionFeature>()
             .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
             // .ForMember(d => d.CreatedBy, m => m.MapFrom(s => s.CreatedBy))
             //.ForMember(d => d.VersionId, m => m.MapFrom(s => s.VersionId))
             //.ForMember(d => d.FeatureId, m => m.MapFrom(s => s.FeatureId))
             //.ForMember(d => d.MoreDetail, m => m.MapFrom(s => s.MoreDetail))
             ;

            #endregion

            #region Application Module

            CreateMap<CreateVersionModuleAPIInputDto, CreateVersionModuleInputDto>()
                .ConvertUsing<CreateVersionModuleTypeConverter>();
            CreateMap<UpdateVersionModuleAPIInputDto, UpdateVersionModuleInputDto>()
              .ConvertUsing<UpdateVersionModuleTypeConverter>();
            CreateMap<AssignModuleVersionsDto, VersionModule>()
             .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
             // .ForMember(d => d.CreatedBy, m => m.MapFrom(s => s.CreatedBy))
             //.ForMember(d => d.VersionId, m => m.MapFrom(s => s.VersionId))
             //.ForMember(d => d.ModuleId, m => m.MapFrom(s => s.ModuleId))
             //.ForMember(d => d.MoreDetail, m => m.MapFrom(s => s.MoreDetail))
             ;

            #endregion

            #region Application AddOn

            CreateMap<CreateVersionAddOnAPIInputDto, CreateVersionAddOnInputDto>()
                .ConvertUsing<CreateVersionAddOnTypeConverter>();
            CreateMap<UpdateVersionAddOnAPIInputDto, UpdateVersionAddOnInputDto>()
              .ConvertUsing<UpdateVersionAddOnTypeConverter>();
            CreateMap<AssignAddOnVersionsDto, VersionAddon>()
             .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
             // .ForMember(d => d.CreatedBy, m => m.MapFrom(s => s.CreatedBy))
             //.ForMember(d => d.VersionId, m => m.MapFrom(s => s.VersionId))
             //.ForMember(d => d.AddonId, m => m.MapFrom(s => s.AddOnId))
             //.ForMember(d => d.MoreDetail, m => m.MapFrom(s => s.MoreDetail))
             ;

            #endregion

            #region Tax
            CreateMap<CreateTaxInputDto, Tax>()
            .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
            .ForMember(dest => dest.TempGuid, opt => opt.MapFrom(c => Guid.NewGuid().ToString()));


            CreateMap<UpdateTaxInputDto, Tax>()
          .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
          .ForMember(pl => pl.CreateDate, options => options.Ignore())
          .ForMember(dest => dest.ModifiedDate, opts => { opts.MapFrom(aa => DateTime.UtcNow); opts.Condition(dto => dto.Id != 0); })
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region Wishlist
            CreateMap<CreateWishlistApplicationInputDto, WishListApplication>()
                .ForMember(Dest => Dest.CreateDate,
                opt => opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<CreateWishlistAddOnInputDto, WishListAddOn>()
               .ForMember(Dest => Dest.CreateDate,
               opt => opt.MapFrom(s => DateTime.UtcNow));
            CreateMap<WishlistAddOnSearchApIInputDto, WishlistAddOnSearchInputDto>();
            CreateMap<WishlistApplicationSearchApiInputDto, WishlistApplicationSearchInputDto>();
            #endregion

            #region ReviewCustomer
            CreateMap<NewReviewDto, CustomerReview>()
               .ForMember(Dest => Dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
               .ForMember(Dest => Dest.RateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
               .ForMember(Dest => Dest.StatusId, opt => opt.MapFrom(s => (int)ReviewStatusEnum.Pending));

            CreateMap<EditReviewDto, CustomerReview>()
               .ForMember(Dest => Dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
               .ForMember(Dest => Dest.RateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
               .ForMember(Dest => Dest.StatusId, opt => opt.MapFrom(s => (int)ReviewStatusEnum.Pending));
            #endregion

            #region ReviewAdmin
            CreateMap<SubmitCustomerReviewInputDto, CustomerReview>()
              .ForMember(Dest => Dest.ApprovedDate,
             opt =>
             opt.MapFrom(s => DateTime.UtcNow));
            CreateMap<SubmitCustomerReviewApiInputDto, SubmitCustomerReviewInputDto>();
            #endregion

            #region Auth.
            CreateMap<PreregisterLandingPageDto, CustomerMobile>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(dest => dest.PhoneCode, opt => opt.MapFrom(c => c.CountryCode))
                /*.AfterMap((src, dest) => { dest.SendCount++; })*/;

            CreateMap<PreregisterLandingPageDto, CustomerEmail>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(pl => pl.Id, src => src.Ignore());

            CreateMap<PreregisterLandingPageDto, Customer>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(c => c.Name))
                .ForMember(dest => dest.CustomerStatusId, opt => opt.MapFrom(c => (int)CustomerStatusEnum.UnVerified))
                .ForMember(dest => dest.TempGuid, opt => opt.MapFrom(c => Guid.NewGuid().ToString()));

            CreateMap<NewCustomerByAdminDto, CustomerMobile>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.PhoneCode, opt => opt.MapFrom(c => c.CountryCode))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(pl => pl.Id, src => src.Ignore());

            CreateMap<NewCustomerByAdminDto, CustomerEmail>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(pl => pl.Id, src => src.Ignore());

            CreateMap<NewCustomerByAdminDto, Customer>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(c => c.Name))
                .ForMember(dest => dest.CustomerStatusId, opt => opt.MapFrom(c => (int)CustomerStatusEnum.Registered))
                .ForMember(dest => dest.TempGuid, opt => opt.MapFrom(c => Guid.NewGuid().ToString()));

            CreateMap<PreregisterDto, CustomerMobile>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(dest => dest.PhoneCode, opt => opt.MapFrom(c => c.MobileCountryCode))
                .AfterMap((src, dest) => { dest.SendCount++; });

            CreateMap<PreregisterDto, Customer>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(dest => dest.CustomerStatusId, opt => opt.MapFrom(c => (int)CustomerStatusEnum.UnVerified))
                .ForMember(dest => dest.TempGuid, opt => opt.MapFrom(c => Guid.NewGuid().ToString()));

            CreateMap<RegisterDto, CustomerEmail>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(c => c.Email))
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => true))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(pl => pl.Id, src => src.Ignore())
                .AfterMap((src, dest) => { dest.SendCount++; });

            CreateMap<RegisterDto, Customer>()
                .ForMember(pl => pl.ModifiedDate, src => src.MapFrom(c => DateTime.UtcNow))
                .ForMember(dest => dest.CustomerStatusId, opt => opt.MapFrom(c => (int)CustomerStatusEnum.Pending))
                .ForMember(pl => pl.Name, src => src.Ignore())
                .ForMember(pl => pl.Password, src => src.Ignore())
                .ForMember(pl => pl.CountryId, src => src.Ignore())
                .ForMember(pl => pl.CreateDate, src => src.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AlternativeEmailDto, CustomerEmail>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => false))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .AfterMap((src, dest) => { dest.SendCount++; });

            CreateMap<AlternativeMobileDto, CustomerMobile>()
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(c => false))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(dest => dest.PhoneCode, opt => opt.MapFrom(c => c.MobileCountryCode))
                .AfterMap((src, dest) => { dest.SendCount++; });
            #endregion

            #region CustomerAccount

            CreateMap<UpdateCustomerInputDto, Customer>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;
            CreateMap<UploadCustomerImageInputDto, Customer>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(pl => pl.Image, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;
            CreateMap<UploadCustomerImageAptInputDto, UploadCustomerImageInputDto>()
                .ForMember(pl => pl.File, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;
            #endregion

            #region PaymentSetup
            CreateMap<CreatePaymentSetupInputDto, PaymentMethod>()
                .ForMember(dest => dest.Credential, opt => opt.MapFrom(s => JsonConvert.SerializeObject(s.PaymentSetupCredential)))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                ;

            CreateMap<UpdatePaymentSetupInputDto, PaymentMethod>()
              .ForMember(dest => dest.Credential, opt => opt.MapFrom(s => JsonConvert.SerializeObject(s.PaymentSetupCredential)))
              .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(s => DateTime.UtcNow))
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;

            #endregion
            #region CustomerAppTags
            CreateMap<FilterCustomerAppTagsApiInputDto, FilterCustomerAppTagsInputDto>();
            #endregion

            #region CustomerProduct
            CreateMap<RefundRequest, RefundRequestInputDto>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<CustomerProductApIInputDto, CustomerProductInputDto>();

            CreateMap<CreateLicenseInputDto, License>()
              .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(s => s.DeviceName))
              .ForMember(dest => dest.Serial, opt => opt.MapFrom(s => s.SerialNumber));

            CreateMap<NewAddOnLicenseDto, License>()
              .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
              .ForMember(dest => dest.LicenseStatusId, opt => opt.MapFrom(s => (int)LicenseStatusEnum.InProgress));

            CreateMap<UpdateLicenseInputDto, RequestChangeDevice>()
                .ForMember(dest => dest.NewDeviceName, opt => opt.MapFrom(s => s.DeviceName))
                .ForMember(dest => dest.NewSerial, opt => opt.MapFrom(s => s.SerialNumber))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.ReasonChangeDeviceId, opt => opt.MapFrom(s => s.ReasonChangeId));

            CreateMap<RequestChangeDeviceFilterDto, LicencesFilterInputDto>();

            CreateMap<NewDeviceDto, License>()
                .ForMember(dest => dest.CustomerSubscriptionId, src => src.MapFrom(l => l.SubscriptionId))
                .ForMember(dest => dest.CreateDate, src => src.MapFrom(l => DateTime.UtcNow))
                .ForMember(dest => dest.ActivateOn, src => src.MapFrom(l => DateTime.UtcNow))
                .ForMember(dest => dest.LicenseStatusId, src => src.MapFrom(l => (int)LicenseStatusEnum.Generated));

            CreateMap<CancelAddonSubscriptionDto, RefundRequest>()
                .ForMember(dest => dest.CreateDate, src => src.MapFrom(r => DateTime.UtcNow))
                .ForMember(dest => dest.RefundRequestStatusId, src => src.MapFrom(r => (int)RefundRequestStatusEnum.Open));

            CreateMap<NewLicenseLogDto, LicenseLog>()
                .ForMember(dest => dest.OldStatusId, src =>
                {
                    src.PreCondition(l => l.OldStatusId.HasValue);
                    src.MapFrom(l => (int)l.OldStatusId);
                })
                .ForMember(dest => dest.NewStatusId, src =>
                {
                    src.PreCondition(l => l.NewStatusId.HasValue);
                    src.MapFrom(l => (int)l.NewStatusId);
                })
                .ForMember(dest => dest.CreatedByAdmin, src =>
                {
                    src.PreCondition(l => l.IsCreatedByAdmin);
                    src.MapFrom(l => l.CreatedBy);
                })
                .ForMember(dest => dest.CreatedByCustomer, src =>
                {
                    src.PreCondition(l => !l.IsCreatedByAdmin);
                    src.MapFrom(l => l.CreatedBy);
                })
                .ForMember(dest => dest.ActionTypeId, src => src.MapFrom(l => (int)l.ActionTypeId))
                .ForMember(dest => dest.CreateDate, src => src.MapFrom(l => DateTime.UtcNow));
            #endregion


            #region ContractCustomer
            CreateMap<FilteredResultRequestDto, InvoiceFilterDto>();
            CreateMap<InvoiceFilterDto, FilterInvoiceByCountry>();

            CreateMap<FilteredResultRequestDto, FilterByEmployeeCountryInputDto>();

            CreateMap<UpdateCustomerByAdminInputDto, Customer>();
            #endregion
            #region Invoice Totals
            CreateMap<CreateInvoiceDetailInputDto, InvoiceDetail>();
            CreateMap<InvoiceTotals, Invoice>()
             .ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(s => s.Details));
            #endregion
            #region Invoices and Subscriptions


            CreateMap<InvoiceDto, Invoice>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                 .ForMember(dest => dest.InvoiceStatusId, opt => opt.MapFrom(s => (int)InvoiceStatusEnum.Unpaid))
                .ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(s => s.Details));
            CreateMap<CreateInvoiceDetailInputDto, InvoiceDetail>();
            CreateMap<CreateSubscriptionInputDto, CustomerSubscription>()
                 .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow));
            CreateMap<VersionSubscriptionInputDto, VersionSubscription>().ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow));
            CreateMap<AddOnSubscriptionInputDto, AddonSubscription>().ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<PayAndSubscribeInputDto, ThirdPartyInvoiceInputDto>()
                 .ForMember(dest => dest.PaymentMethodId, opt => opt.MapFrom(s => s.Invoice.PaymentMethodId))
                 ;

            CreateMap<APIPayAndSubscribeInputDto, PayAndSubscribeInputDto>()
                 .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))

            ;
            CreateMap<APIPayAndSubscribeInputDto, CustomerSubscription>()
         .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
         .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.Id))
         ;
            CreateMap<APIPayAndSubscribeInputDto, VersionSubscription>()
        .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
         .ForMember(dest => dest.Id, opt => opt.Ignore()/*.MapFrom(s => s.VersionSubscriptionId)*/)
           .ForMember(dest => dest.VersionPrice, opt => opt.Ignore()/*.MapFrom(s => s.VersionSubscriptionId)*/)
            .ForMember(dest => dest.VersionName, opt => opt.MapFrom(s => s.VersionTitle))

         ;
            CreateMap<APIPayAndSubscribeInputDto, AddonSubscription>()
       .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
        .ForMember(dest => dest.AddonName, opt => opt.MapFrom(s => s.AddonTitle))
       .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.AddOnSubscriptionId.GetValueOrDefault(0)));

            CreateMap<APIPayAndSubscribeInputDto, InvoiceDto>()
      .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
         //.ForMember(dest => dest.CustomerSubscriptionId, opt => opt.MapFrom(s => s.CustomerSubscriptionId))
         .ForMember(dest => dest.StartDate, opt => opt.MapFrom(s => DateTime.UtcNow))
       .ForMember(dest => dest.EndDate, opt => opt.MapFrom(s => DateTime.UtcNow.AddDays(s.RenewEvery)))
         //TODO:Remove this
         .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(s => DateTime.UtcNow))
         .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.InvoiceId));

            CreateMap<RetrieveInvoiceDto, InvoiceDto>()
    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
       .ForMember(dest => dest.CustomerSubscriptionId, opt => opt.MapFrom(s => s.CustomerSubscription.Id))
       //.ForMember(dest => dest.InvoiceStatusId, opt => opt.MapFrom(s => (int)InvoiceStatusEnum.Unpaid))
       .ForMember(dest => dest.StartDate, opt => opt.MapFrom(s => DateTime.UtcNow))
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateInvoiceDetailInputDto, InvoiceDetail>()
         .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;

            CreateMap<APIPayAndSubscribeInputDto, ThirdPartyInvoiceInputDto>();
            CreateMap<APIPayAndSubscribeInputDto, GetPayAndSubscribeOutputDto>();

            #endregion
            #region InvoiceAdmin
            CreateMap<CreateAdminInvoiceInputDto, PaymentDetailsInputDto>();
            CreateMap<UpdateAdminInvoiceInputDto, PaymentDetailsInputDto>();
            #endregion
            CreateMap<GetCustomerDefaultTaxApiInputDto, GetCustomerDefaultTaxInputDto>();


            #region Role
            CreateMap<CreateRoleInputDto, Role>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.RolePageActions, opt => opt.MapFrom(s => s.RolePageAction))
                   .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
            CreateMap<RolePageActionDto, RolePageAction>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                //.ForMember(dest => dest.PageAction, opt => opt.MapFrom(s => s.PageAction))
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
            CreateMap<PageActionDto, PageAction>();
            CreateMap<PageDto, Page>();
            CreateMap<ActionDto, Action>();
            #endregion
            #region CustomerCard
            CreateMap<CardTokenDto, CustomerCard>();
            #endregion

            #region WorkSpaces
            CreateMap<CreateWorkSpaceDto, Workspace>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(s => DateTime.Now))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(s => DateTime.Now.AddDays(14)))
                ;
            CreateMap<CreatWorkSpaceApiDto, CreateWorkSpaceDto>();

            CreateMap<UpdateWorkSpaceDto, Workspace>();
            CreateMap<UpdateWorkSpaceApiDto, UpdateWorkSpaceDto>()
                .ForMember(dest => dest.ConnectionString, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(s => DateTime.Now))
                ;
            CreateMap<SimpleDatabaseDto, SimpleDatabas>();

            #endregion

            #region Ticket 
            CreateMap<CreateTicketDto, Ticket>()
                .ForMember(dest => dest.CreateDate, opts => opts.MapFrom(s => DateTime.UtcNow))
                .ForMember(dest => dest.StatusId, opts => opts.MapFrom(s =>(int)TableStatusEnum.Open));
            #endregion


            #region ChatMesasge

            CreateMap<MessageInputDto, MessageDto>()
     .ForMember(dest => dest.Files, opts => opts.MapFrom(s => s.Files))
     .ForMember(dest => dest.TicketId, opts => opts.MapFrom(s => s.TicketId))
     .ForMember(dest => dest.HasAttachment, opts => opts.MapFrom(s => s.HasAttachment))
     .ForMember(dest => dest.Message, opts => opts.MapFrom(s => s.Message))

     ;
            CreateMap<MessageDto, ChatMessage>()
     .ForMember(dest => dest.SendTime, opts => opts.MapFrom(s => DateTime.UtcNow))
     .ForMember(dest => dest.TicketId, opts => opts.MapFrom(s => s.TicketId))
     .ForMember(dest => dest.HasAttachment, opts => opts.MapFrom(s => s.HasAttachment))
     .ForMember(dest => dest.Message, opts => opts.MapFrom(s => s.Message))
     .ForMember(dest => dest.IsCustomer, opts => opts.MapFrom(s => s.UserType == ChatRecieverTypeEnum.Customer))

     ;

            #endregion
        }
    }
}
