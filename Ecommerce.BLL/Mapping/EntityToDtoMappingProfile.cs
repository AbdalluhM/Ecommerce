using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Customers;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.License;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Addons.AddonBase.Inputs;
using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonPrice;
using Ecommerce.DTO.Addons.AddonSliders.Outputs;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications.ApplicationBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationModules;
using Ecommerce.DTO.Applications.ApplicationSlider;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Applications.VersionAddOns;
using Ecommerce.DTO.Applications.VersionFeatures;
using Ecommerce.DTO.Applications.VersionModules;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Contracts.Licenses.Outputs;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Customers.Auth.Outputs;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.CustomerProduct;
using Ecommerce.DTO.Customers.DevicesType;
using Ecommerce.DTO.Customers.DownloadCenter;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using Ecommerce.DTO.Employees;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Logs;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Modules;
using Ecommerce.DTO.Modules.ModuleBase.Outputs;
using Ecommerce.DTO.Modules.ModuleSlider;
using Ecommerce.DTO.Modules.ModuleTags;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;
using Ecommerce.DTO.Taxes;
using Ecommerce.DTO.WorkSpaces;
using Ecommerce.Reports.Templts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Contracts.Licenses.LicensesDto;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;
using static Ecommerce.DTO.Customers.HomePage.HomePageDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;
using static Ecommerce.DTO.Roles.RoleDto;
using Action = Ecommerce.Core.Entities.Action;
using FileStorageDto = Ecommerce.DTO.Settings.Files.FileStorageDto;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Mapping
{
    public class EntityToDtoMappingProfile : Profile
    {
        public EntityToDtoMappingProfile()
        {
            #region Notifications
            //CreateMap<Notification, GetNotificationOutputDto>()
            //.ConvertUsing<GetNotificationOutputDtoTypeConverter>();
            CreateMap<Notification, GetNotificationOutputDto>()
                 .ForMember(dest => dest.VersionSubscribtionId, opts =>
                 opts.MapFrom(src =>
                 src.Invoice != null && !src.Invoice.CustomerSubscription.IsAddOn && src.Invoice.CustomerSubscription.VersionSubscriptions.Count() > 0 ? src.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().Id :
                 src.Licence != null && !src.Licence.CustomerSubscription.IsAddOn && src.Licence.CustomerSubscription.VersionSubscriptions.Count() > 0 ? src.Licence.CustomerSubscription.VersionSubscriptions.FirstOrDefault().Id : 0
                 ))
                 .ForMember(dest => dest.AddonSubscribtionId, opts =>
                 opts.MapFrom(src =>
                 src.Invoice != null && src.Invoice.CustomerSubscription.IsAddOn && src.Invoice.CustomerSubscription.AddonSubscriptions.Count() > 0 ? src.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().Id :
                 src.Licence != null && src.Licence.CustomerSubscription.IsAddOn && src.Licence.CustomerSubscription.AddonSubscriptions.Count() > 0 ? src.Licence.CustomerSubscription.AddonSubscriptions.FirstOrDefault().Id : 0
                 ))
                 .ForMember(dest => dest.CustomerName, opts => opts.MapFrom(src => src.Customer != null ? src.Customer.Name : null))
                 .ForMember(dest => dest.ActionTypeName, opts => opts.MapFrom(src => src.NotificationAction.NotificationActionType.Name))
                 .ForMember(dest => dest.CreatedByEmployeeUserName, opts => opts.MapFrom(src =>
                 src.NotificationAction.IsCreatedBySystem ? "System" : src.NotificationAction.IsAdminSide ?
                 (src.Customer != null ? src.Customer.Name : null)
                 : (src.CreatedByEmployee != null ? src.CreatedByEmployee.UserName : null)))
                  .ForMember(dest => dest.ReadByEmployeeUserName, opts => opts.MapFrom(src => src.ReadByEmployee != null ? src.ReadByEmployee.UserName : null))
                   .ForMember(dest => dest.HiddenByEmployeeUserName, opts => opts.MapFrom(src => src.HiddenByEmployee != null ? src.HiddenByEmployee.UserName : null))
                    .ForMember(dest => dest.PageDetailId, opts => opts.MapFrom(src => src.NotificationAction.NotificationActionPageId.Value))
                .ForMember(dest => dest.Description, opts =>
                opts.MapFrom<NotificationActionDescriptionResolver, string>(src => src.NotificationAction.Description));

            CreateMap<FilteredResultRequestDto, NotificationFilteredResultRequestDto>();
            CreateMap<NewTicketDto, NewTicketNotificationDto>();
            CreateMap<NewTicketMessageDto, NewTicketMessageNotificationDto>();
            #endregion

            #region Employees
            CreateMap<Employee, GetEmployeeOutputDto>()
                .ForMember(dest => dest.Role, opts => opts.MapFrom(s => s.Role.Name))
                .ForMember(dest => dest.Image
                   , opts => opts.MapFrom(s => s.Image))
                .ForMember(dest => dest.EmployeeCountries, opts =>
                opts.MapFrom(s => s.EmployeeCountries.Where(x => x.CountryCurrency != null)));
            CreateMap<EmployeeCountry, UpdateAssignEmployeeToCountryInputDto>();

            CreateMap<EmployeeCountry, GetAssignedEmployeeToCountryOutputDto>()
                      .ForMember(dest => dest.EmployeeUserName, opts => opts.MapFrom(s => s.Employee.UserName))
                .ForMember(dest => dest.EmployeeEmail, opts => opts.MapFrom(s => s.Employee.Email))
                  .ForMember(dest => dest.EmployeeName, opts => opts.MapFrom(s => s.Employee.Name))
                .ForMember(dest => dest.CountryName, opts => opts.MapFrom(s => s.CountryCurrency.Country.Name))
                .ForMember(dest => dest.CurrencyId, opts => opts.MapFrom(s => s.CountryCurrency.Currency.Id))
                .ForMember(dest => dest.CurrencyName, opts => opts.MapFrom(s => s.CountryCurrency.Currency.Name))
                .ForMember(dest => dest.CurrencySymbol, opts => opts.MapFrom(s => s.CountryCurrency.Currency.Symbole));

            CreateMap<GetEmployeeOutputDto, LoginModelOutputDto>()
            .ForMember(dest => dest.Role, opts => opts.MapFrom(s => s.Role))
             .ForMember(dest => dest.RoleId, opts => opts.MapFrom(s => s.RoleId));
            #endregion

            #region Countries

            CreateMap<Country, GetCountryOutputDto>();
            CreateMap<CountryCurrency, GetCountryOutputDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(s => s.CountryId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty))
                  .ForMember(dest => dest.PhoneCode, opts => opts.MapFrom(s => s.Country != null ? s.Country.PhoneCode : string.Empty))
                  .ForMember(dest => dest.IsActive, opts => opts.MapFrom(s => s.Country != null ? s.Country.IsActive : false))
          .ForMember(dest => dest.Taxes, opts => opts.MapFrom(s => s.Country.Taxes));

            CreateMap<Country, GetAssignedCurrencyToCountryOutputDto>()
          //      .ForMember(dest => dest.CountryId, opts => opts.MapFrom(s => s.Id))
          //.ForMember(dest => dest.CountryName, opts => opts.MapFrom(s => s.Name))
          // .ForMember(dest => dest.CurrencyId, opts => opts.MapFrom(s => s.CountryCurrencies.Where(x => x.CountryId == s.Id).FirstOrDefault().Currency.Id))
          //.ForMember(dest => dest.CurrencyName, opts => opts.MapFrom(s => s.CountryCurrencies.Where(x => x.CountryId == s.Id).FirstOrDefault().Currency.Name))
          //.ForMember(dest => dest.CurrencySymbol, opts => opts.MapFrom(s => s.CountryCurrencies.Where(x => x.CountryId == s.Id).FirstOrDefault().Currency.Symbole))
          .ForMember(dest => dest.Country, opts => opts.MapFrom(s => s))
           .ForMember(dest => dest.Currency, opts => opts.MapFrom(s => s.CountryCurrency.Currency))

          // Todo-Saeed: Country currency compile time error.
          .ForMember(dest => dest.Country, opts => opts.MapFrom(s => s))
           .ForMember(dest => dest.Currency, opts => opts.MapFrom(s => s.CountryCurrency.Currency))

          .ForMember(dest => dest.Taxes, opts => opts.MapFrom(s => s.Taxes));

            CreateMap<CountryCurrency, GetAssignedCurrencyToCountryOutputDto>()
            .ForMember(dest => dest.Country, opts => opts.MapFrom(s => s.Country))
            .ForMember(dest => dest.Currency, opts => opts.MapFrom(s => s.Currency))
            .ForMember(dest => dest.CountryName, opts => opts.MapFrom(s => s.Country.Name))
            .ForMember(dest => dest.CurrencyName, opts => opts.MapFrom(s => s.Currency.Name))
            .ForMember(dest => dest.CurrencySymbol, opts => opts.MapFrom(s => s.Currency.Symbole))
            .ForMember(dest => dest.Taxes, opts => opts.MapFrom(s => s.Country.Taxes));

            #endregion

            #region Contact Us.
            CreateMap<ContactUsHelpOption, ContactUsLookupDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(h => h.HelpName));
            CreateMap<Industry, ContactUsLookupDto>();
            CreateMap<CompanySize, ContactUsLookupDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(c => c.Size));
            CreateMap<DexefBranch, DexefBranchDto>()
                .ForMember(dest => dest.Country, src => src.MapFrom(b => b.Country.Name));
            #endregion

            #region Currency
            CreateMap<Currency, GetCurrencyOutputDto>().ForMember(dest => dest.Symbol, opts => opts.MapFrom(s => s.Symbole));
            #endregion

            #region DashBoard View
            CreateMap<ViewDashboardTotal, GetDashBoardOutputDto>();

            #endregion

            #region SubscriptionType
            CreateMap<SubscriptionType, GetSubscriptionTypeOutputDto>();
            #endregion

            #region Employee
            CreateMap<EmployeeType, GetEmployeeTypeOutputDto>();

            #endregion

            #region Tags
            CreateMap<Tag, GetTagOutputDto>();

            //CreateMap<AddOn, GetAddOnOutputDto>()
            //    .ForMember(dest => dest.Label
            //   , opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
            //    .ForMember(dest => dest.MissingPricesCount, opts => opts.Ignore());

            #endregion

            #region AddonTags

            CreateMap<AddOnTag, GetAddonTagOutputDto>()
                .ForMember(h => h.AddonTagId, d => d.MapFrom(d => d.Id))
            .ForMember(h => h.Name, d => d.MapFrom(d => d.Tag.Name));

            //CreateMap<AddOnTag, SelectAddonTagDto>()
            //.ForMember(h => h.AddonTagId, d => d.MapFrom(d => d.Id))
            //.ForMember(h => h.Name, d => d.MapFrom(d => d.Tag.Name));

            #endregion

            #region AddOn
            CreateMap<AddOn, GetAddOnOutputDto>()
                .ForMember(dest => dest.Label
               , opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
                .ForMember(dest => dest.MissingPricesCount, opts =>
                   opts.MapFrom<AddOnMissingPricesCountResolver, int>(src => src.Id));

            CreateMap<AddOn, AddonDetailsDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Logo/*.FullPath*/))
                .ForMember(dest => dest.Description, src => src.MapFrom(a => a.LongDescription))
                //
                .ForMember(dest => dest.ShortDescription, src => src.MapFrom(a => a.ShortDescription))
                .ForMember(dest => dest.AddonLabel, src => src.MapFrom(a => a.AddOnLabels.FirstOrDefault()))
                .ForMember(dest => dest.SlidersPath, src => src.MapFrom<CustomListUrlsResolver, List<string>>(a =>
                    a.AddOnSliders.Any() ? a.AddOnSliders.Select(s => s.Media.FullPath).ToList() : null))
                .ForMember(dest => dest.Tags, src => src.MapFrom(a => a.AddOnTags.Where(e => e.Tag != null).Select(t => t.Tag)));
            //.ForMember(dest => dest.AddonPrice, src => src.MapFrom<AddonMinimumPriceResolver, int>(a => a.Id));

            CreateMap<AddOn, AppAddonInfoDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Logo/*.FullPath*/));
            //CreateMap<AddOn, GetAddOnDataOutputDto>()
            //      .ForMember(dest => dest.Id, src => src.MapFrom(a => a.Id))
            //      .ForMember(dest => dest.AddOnPrices, src => src.MapFrom(a => a.AddOnPrices));

            //CreateMap<GetAddOnDataOutputDto, AddOnForBuyNowOutputDto>()
            //    .ForMember(dest => dest.AddOnId, src => src.MapFrom(a => a.Id))
            //    .ForMember(dest => dest.AddOnImage, src => src.MapFrom(a => a.Logo))
            //    .ForMember(dest => dest.AddOnName, src => src.MapFrom(a => a.Name))
            //    .ForMember(dest => dest.AddOnTitle, src => src.MapFrom(a => a.Title))
            //    .ForMember(dest => dest.AddOnPrice, src => src.MapFrom(a => a.MinAddOnPrice))
            //    ;


            #endregion

            #region AddonLabels

            CreateMap<AddOnLabel, GetAddonLabelOutputDto>();
            CreateMap<AddOnLabel, AddonLabelDto>();
            #endregion

            #region AddOnPrices

            CreateMap<AddOnPrice, GetAddOnPriceOutputDto>()
                .ForMember(dest => dest.CountryName
               , opts => opts.MapFrom(s => s.CountryCurrency.Country.Name))
                .ForMember(dest => dest.PriceLevelName
               , opts => opts.MapFrom(s => s.PriceLevel.Name))
                                .ForMember(dest => dest.NumberOfLicenses
               , opts => opts.MapFrom(s => s.PriceLevel.NumberOfLicenses))
               .ForMember(dest => dest.CurrencyShortCode
               , opts => opts.MapFrom(s => s.CountryCurrency.Currency.Symbole));

            CreateMap<ViewMissingAddOnPrice, GetAddOnPriceOutputDto>()
              .ForMember(dest => dest.CountryName
             , opts => opts.MapFrom(s => s.CountryName))
              .ForMember(dest => dest.PriceLevelName
             , opts => opts.MapFrom(s => s.PriceLevelName))
                              .ForMember(dest => dest.NumberOfLicenses
             , opts => opts.MapFrom(s => s.NumberOfLicenses))
             .ForMember(dest => dest.CurrencyShortCode
             , opts => opts.MapFrom(s => s.CurrencyShortCode));
            CreateMap<EmployeeCountry, UpdateAssignEmployeeToCountryInputDto>();

            CreateMap<AddOnPrice, AddonPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(ap => ap.CountryCurrency.CountryId))
                .ForMember(dest => dest.PriceBeforeDiscount, src => src.Ignore())
                .ForMember(dest => dest.DiscountPercentage, src => src.Ignore())
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(ap => Math.Min(Math.Min(ap.MonthlyNetPrice, ap.ForeverNetPrice), ap.YearlyNetPrice)))
                .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(ap => ap.CountryCurrency.Currency.Code))
                .AfterMap((src, dest) =>
                {
                    // Find the minimum net price
                    var minNetPrice = Math.Min(Math.Min(src.MonthlyNetPrice, src.ForeverNetPrice), src.YearlyNetPrice);

                    // Set NetPrice
                    dest.NetPrice = minNetPrice;

                    // Determine which price and discount to use based on the minimum net price
                    if (minNetPrice == src.MonthlyNetPrice)
                    {
                        dest.PriceBeforeDiscount = src.MonthlyPrice;
                        dest.DiscountPercentage = src.MonthlyPrecentageDiscount;
                        dest.Discrimination = "Monthly";
                    }
                    else if (minNetPrice == src.YearlyNetPrice)
                    {
                        dest.PriceBeforeDiscount = src.YearlyPrice;
                        dest.DiscountPercentage = src.YearlyPrecentageDiscount;
                        dest.Discrimination = "Yearly";

                    }
                    else // minNetPrice == src.ForeverNetPrice
                    {
                        dest.PriceBeforeDiscount = src.ForeverPrice;
                        dest.DiscountPercentage = src.ForeverPrecentageDiscount;
                        dest.Discrimination = "Forever";

                    }
                })
                ;


            CreateMap<AddOnPrice, MonthlyPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
                .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.MonthlyPrice))
                .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.MonthlyPrecentageDiscount))
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.MonthlyNetPrice))
                .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Others.GetDescription()));

            CreateMap<AddOnPrice, YearlyPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
             .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.YearlyPrice))
             .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.YearlyPrecentageDiscount))
             .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.YearlyNetPrice))
             .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
            /* .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Yearly.GetDescription()))*/;

            CreateMap<AddOnPrice, ForeverPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
                .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.ForeverPrice))
                .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.ForeverPrecentageDiscount))
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.ForeverNetPrice))
                .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Forever.GetDescription()));
            CreateMap<AddOnPrice, AddOnPriceData>()
             .ForMember(dest => dest.Price, src => src.MapFrom<AddonPriceResolver, AddOnPriceDto>(v => new AddOnPriceDto { AddOnId = v.AddOnId, PriceLevelId = v.PriceLevelId, CountryCurrencyId = v.CountryCurrencyId }))
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<AddOn, GetAddOnDataOutputDto>()
                .ForMember(dest => dest.Logo
     , opts => opts.MapFrom(s => s.Logo))
                      .ForMember(dest => dest.AddOnPrices
     , opts => opts.MapFrom(s => s.AddOnPrices))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<GetAddOnDataOutputDto, AddOnForBuyNowOutputDto>()

        .ForMember(dest => dest.VersionId
     , opts => opts.Ignore())
         .ForMember(dest => dest.ApplicationId
     , opts => opts.Ignore())
              .ForMember(dest => dest.ApplicationName
     , opts => opts.Ignore())
        .ForMember(dest => dest.AddOnImage
     , opts => opts.MapFrom(s => s.Logo))
      // .ForMember(dest => dest.AddOnName
      //, opts => opts.MapFrom(s => s.Name))
      .ForMember(dest => dest.AddOnTitle
     , opts => opts.MapFrom(s => s.Title))
      .ForMember(dest => dest.AddOnId
     , opts => opts.MapFrom(s => s.Id))
     .ForMember(dest => dest.AddOnPrice, src => src.MapFrom<AddonPriceResolver, AddOnPriceDto>(v => v.MinAddOnPrice))
  .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region AddOn Sliders
            CreateMap<AddOnSlider, RetrieveAddonSliderDto>()
           .ForMember(dest => dest.Image,
        opt => opt.MapFrom(src => src.Media))

                .ForMember(sDto => sDto.Name, slider => slider.MapFrom(s => s.Media.RealName))
                .ForMember(sDto => sDto.Size, slider => slider.MapFrom(s => s.Media.FileSize));
            #endregion

            #region FileStorage
            CreateMap<FileStorage, FileStorageDto>()
                 .ForMember(dest => dest.FullPath, opt => opt.MapFrom<CustomAdminURLResolver, string>(src => src.FullPath))
                .ForMember(dest => dest.FileType, opts => opts.MapFrom(s => s.FileType.Name));

            CreateMap<FileStorage, FileStorageBlobDto>()
                .ForMember(dest => dest.FullPath, opt => opt.MapFrom<CustomAdminBlobURLResolver, string>(src => src.FullPath))
               .ForMember(dest => dest.FileType, opts => opts.MapFrom(s => s.FileType.Name));


            #endregion

            #region Features
            CreateMap<Feature, GetFeatureOutputDto>();
            CreateMap<Feature, GetFeatureDropDownOutputDto>();
            #endregion

            #region Price Levels
            CreateMap<PriceLevel, RetrievePriceLevelDto>();
            CreateMap<PriceLevel, GetApplicationPackagesOutputDto>()
           .ForMember(dest => dest.Details, src => src.MapFrom(vp => vp.VersionPrices));

            #endregion

            #region Module
            CreateMap<Module, GetModuleOutputDto>()
              .ForMember(dest => dest.Logo
               , opts => opts.MapFrom(s => s.Image));

            CreateMap<Module, AppModuleInfoDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(m => m.Image));

            CreateMap<Module, AppModulePopupDto>()
                .ForMember(dest => dest.Description, src => src.MapFrom(m => m.LongDescription))
                .ForMember(dest => dest.SliderPath, src => src.MapFrom<CustomAdminURLResolver, string>(m =>
                    m.ModuleSliders.FirstOrDefault() != null ? m.ModuleSliders.FirstOrDefault().Media.FullPath : null));

            CreateMap<Module, ModuleDetailsDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(m => m.Image))
                .ForMember(dest => dest.Description, src => src.MapFrom(a => a.LongDescription))
                .ForMember(dest => dest.SlidersPath, src => src.MapFrom<CustomListUrlsResolver, List<string>>(m =>
                    m.ModuleSliders.Any() ? m.ModuleSliders.Select(s => s.Media.FullPath).ToList() : null))
                .ForMember(dest => dest.Tags, src => src.MapFrom(m => m.ModuleTags.Select(t => t.Tag)));
            #endregion

            #region Module Tags

            CreateMap<ModuleTag, GetModuleTagOutputDto>()
            .ForMember(h => h.Name, d => d.MapFrom(d => d.Tag.Name));
            #endregion

            #region Module Sliders
            CreateMap<ModuleSlider, GetModuleSliderOutputDto>()
            //.ForMember(sDto => sDto.ImagePath, slider => slider.MapFrom(s => s.Media.FullPath))
           .ForMember(dest => dest.Image,
                   opt => opt.MapFrom(src => src.Media))
                .ForMember(sDto => sDto.Name, slider => slider.MapFrom(s => s.Media.RealName))
            .ForMember(sDto => sDto.Size, slider => slider.MapFrom(s => s.Media.FileSize));

            #endregion

            #region Application
            CreateMap<Application, GetApplicationOutputDto>()
               .ForMember(dest => dest.Label, opts => opts.MapFrom(s => s.ApplicationLabels.FirstOrDefault()))
               .ForMember(dest => dest.MissingPricesCount, opts => opts.MapFrom<ApplicationMissionPricesCountResolver, int>(src => src.Id))
               .ForMember(dest => dest.ModulesCount, opts => opts.MapFrom<ApplicationModulesCountResolver, int>(src => src.Id))
               .ForMember(dest => dest.VersionsCount, opts => opts.MapFrom<ApplicationVersionsCountResolver, int>(src => src.Id))
               .ForMember(dest => dest.Logo, opts => opts.MapFrom(s => s.Image));


            CreateMap<Application, GetApplicationDropDownOutputDto>()
                .ForMember(dest => dest.SubscriptionType
                   , opts => opts.MapFrom(s => s.SubscriptionType));

            CreateMap<Application, AppDetailsDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Image/*.FullPath*/))
                .ForMember(dest => dest.Description, src => src.MapFrom(a => a.LongDescription))

                //     .ForMember(dest => dest.countryCurrency,
                //opts => opts.MapFrom(a =>
                //    a.Versions
                //    .SelectMany(v => v.VersionPrices.Select(vp => vp.CountryCurrency.Currency.Code))
                //    .FirstOrDefault()))


                .ForMember(dest => dest.Label, src => src.MapFrom(a => a.ApplicationLabels.FirstOrDefault()))
                .ForMember(dest => dest.SlidersPath, src => src.MapFrom<CustomListUrlsResolver, List<string>>(a =>
                    a.ApplicationSliders.Any() ? a.ApplicationSliders.Select(s => s.Media.FullPath).ToList() : null))
                .ForMember(dest => dest.Tags, src => src.MapFrom(a => a.ApplicationTags.Where(e => e.Tag != null).Select(t => t.Tag)));

            CreateMap<Application, RelatedAppDto>()
                .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Image))
                .ForMember(dest => dest.Label, src => src.MapFrom(a => a.ApplicationLabels.FirstOrDefault()))
                .ForMember(dest => dest.FeaturedTag, src => src.MapFrom(a => a.ApplicationTags.FirstOrDefault(t => t.IsFeatured).Tag.Name))
                .ForMember(dest => dest.Price, src => src.MapFrom<ApplicationMinimumPriceResolver, int>(a => a.Id));

            #endregion

            #region ApplicationLabel
            CreateMap<ApplicationLabel, GetApplicationLabelOutputDto>();
            #endregion

            #region Application Tags

            CreateMap<ApplicationTag, GetApplicationTagOutputDto>()
            .ForMember(h => h.Name, d => d.MapFrom(d => d.Tag != null ? d.Tag.Name : String.Empty));
            #endregion

            #region Application Sliders
            CreateMap<ApplicationSlider, GetApplicationSliderOutputDto>()
                .ForMember(dest => dest.Image,
        opt => opt.MapFrom(src => src.Media))
            .ForMember(sDto => sDto.Name, slider => slider.MapFrom(s => s.Media.RealName))
            .ForMember(sDto => sDto.Size, slider => slider.MapFrom(s => s.Media.FileSize));

            #endregion

            #region Application Versions
            CreateMap<VersionRelease, VersionReleaseOutputDto>();

            CreateMap<Version, GetVersionOutputDto>()
            .ForMember(dest => dest.Logo
               , opts => opts.MapFrom(s => s.Image))
            .ForMember(dest => dest.VersionRelease
               , opts => opts.MapFrom(s => s.VersionReleases.Where(x => x.IsCurrent).FirstOrDefault()))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Version, BaseLookupDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(v => v.Title));

            CreateMap<Version, VersionDetailsDto>()
                .ForMember(dest => dest.DownloadCount, opt => opt.MapFrom(s => s.DownloadCount))
                .ForMember(dest => dest.Addons,
                    src => src.MapFrom(v => v.VersionAddons.Where(x => x.Addon != null).Select(va => va.Addon)))
                .ForMember(dest => dest.Modules,
                    src => src.MapFrom(v => v.VersionModules.Where(x => x.Module != null).Select(va => va.Module)))
                .ForMember(dest => dest.Price, src => src.MapFrom<VersionMinimumPriceResolver, int>(v => v.Id))
                .ForMember(dest => dest.DownloadUrl
                    , opts => opts.MapFrom(s => s.VersionReleases.FirstOrDefault(x => x.IsCurrent).DownloadUrl));

            CreateMap<VersionFeature, VersionFeaturesDto>()
                 .ForMember(dest => dest.Name, src => src.MapFrom(v => v.Version.Name))
                 .ForMember(dest => dest.FetureName, src => src.MapFrom(v => v.Feature.Name))
                 ;





            CreateMap<GetVersionDataOutputDto, GetVersionsByApplicationAndPricingOutputDto>()
                .ForMember(dest => dest.AvailabeFeatures, src => src.MapFrom(v => v.VersionFeatures))
                 .ForMember(dest => dest.Image, src => src.MapFrom(a => a.Logo))
                 .ForMember(dest => dest.Description, src => src.MapFrom(a => a.ShortDescription))
       .ForMember(dest => dest.VersionRelease
            , opts => opts.MapFrom(s => s.VersionRelease))
             .ForMember(dest => dest.Price, src => src.MapFrom<ApplicationVersionPriceResolver, VersionPriceDto>(v => v.MinVersionPrice))
          .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            CreateMap<Version, GetVersionDataOutputDto>()
           .ForMember(dest => dest.VersionFeatures, src => src.MapFrom(v => v.VersionFeatures))
            .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Image))
              .ForMember(dest => dest.ApplicationTitle, src => src.MapFrom(a => a.Application.Title))
     .ForMember(dest => dest.VersionRelease
        , opts => opts.MapFrom(s => s.VersionReleases.Where(x => x.IsCurrent).FirstOrDefault()))
                 .ForMember(dest => dest.SubscriptionTypeId
       , opts => opts.MapFrom(s => s.Application.SubscriptionTypeId))
     .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<GetVersionDataOutputDto, VersionForBuyNowOutputDto>()
         .ForMember(dest => dest.VersionRelease
       , opts => opts.MapFrom(s => s.VersionRelease))
         .ForMember(dest => dest.ReleaseNumber
       , opts => opts.MapFrom(s => s.ReleaseNumber))
          .ForMember(dest => dest.VersionId
       , opts => opts.MapFrom(s => s.Id))
          .ForMember(dest => dest.VersionImage
       , opts => opts.MapFrom(s => s.Logo))
        .ForMember(dest => dest.VersionName
       , opts => opts.MapFrom(s => s.Name))
        .ForMember(dest => dest.VersionTitle
       , opts => opts.MapFrom(s => s.Title))
        .ForMember(dest => dest.ApplicationId
       , opts => opts.MapFrom(s => s.ApplicationId))
              .ForMember(dest => dest.ApplicationTitle
       , opts => opts.MapFrom(s => s.ApplicationTitle))
          .ForMember(dest => dest.SubscriptionTypeId
       , opts => opts.MapFrom(s => s.SubscriptionTypeId))
       .ForMember(dest => dest.VersionPrice, src => src.MapFrom<ApplicationVersionPriceResolver, VersionPriceDto>(v => v.MinVersionPrice))
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            #endregion

            #region Application Version Prices

            CreateMap<VersionPrice, GetVersionPriceOutputDto>()
                .ForMember(dest => dest.CountryName
               , opts => opts.MapFrom(s => s.CountryCurrency.Country.Name))
                .ForMember(dest => dest.PriceLevelName
               , opts => opts.MapFrom(s => s.PriceLevel.Name))
                 .ForMember(dest => dest.NumberOfLicenses
               , opts => opts.MapFrom(s => s.PriceLevel.NumberOfLicenses))
               .ForMember(dest => dest.CurrencyShortCode
               , opts => opts.MapFrom(s => s.CountryCurrency.Currency.Symbole))
                .ForMember(dest => dest.VersionName
               , opts => opts.MapFrom(s => s.Version != null ? s.Version.Name : String.Empty))
                .ForMember(dest => dest.ApplicationId
               , opts => opts.MapFrom(s => s.Version != null ? s.Version.ApplicationId : 0))
                .ForMember(dest => dest.ApplicationName
               , opts => opts.MapFrom(s => s.Version != null && s.Version.Application != null ? s.Version.Application.Name : String.Empty))
                 .ForMember(dest => dest.SubscriptionTypeId
               , opts => opts.MapFrom(s => s.Version != null && s.Version.Application != null ? s.Version.Application.SubscriptionTypeId : 0));




            CreateMap<ViewMissingVersionPrice, GetVersionPriceOutputDto>()
              .ForMember(dest => dest.CountryName
             , opts => opts.MapFrom(s => s.CountryName))
              .ForMember(dest => dest.PriceLevelName
             , opts => opts.MapFrom(s => s.PriceLevelName))
                              .ForMember(dest => dest.NumberOfLicenses
             , opts => opts.MapFrom(s => s.NumberOfLicenses))
             .ForMember(dest => dest.CurrencyShortCode
             , opts => opts.MapFrom(s => s.CurrencyShortCode))
               .ForMember(dest => dest.SubscriptionTypeId
               , opts => opts.MapFrom(s => s.SubscriptionTypeId));

            CreateMap<VersionPrice, MonthlyPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
                .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.MonthlyPrice))
                .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.MonthlyPrecentageDiscount))
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.MonthlyNetPrice))
                .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Others.GetDescription()));

            CreateMap<VersionPrice, YearlyPriceDetailsDto>()
                  .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
             .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.YearlyPrice))
             .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.YearlyPrecentageDiscount))
             .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.YearlyNetPrice))
             .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
            /* .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Yearly.GetDescription()))*/;

            CreateMap<VersionPrice, ForeverPriceDetailsDto>()
                .ForMember(dest => dest.PaymentCountryId, src => src.MapFrom(vp => vp.CountryCurrency.CountryId))
                .ForMember(dest => dest.PriceBeforeDiscount, src => src.MapFrom(vp => vp.ForeverPrice))
                .ForMember(dest => dest.DiscountPercentage, src => src.MapFrom(vp => vp.ForeverPrecentageDiscount))
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(vp => vp.ForeverNetPrice))
                .ForMember(dest => dest.CurrencySymbol, src => src.MapFrom(vp => vp.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.Discrimination, src => src.MapFrom(vp => SubscriptionTypeEnum.Forever.GetDescription()));



            CreateMap<VersionPrice, GetApplicationPackageDetailsoutputDto>()
         .ForMember(dest => dest.VersionId, src => src.MapFrom(vp => vp.VersionId))
        .ForMember(dest => dest.VersionReleaseId, src => src.MapFrom(vp => vp.Version != null && vp.Version.VersionReleases.Any() ? vp.Version.VersionReleases.Where(r => r.IsCurrent).FirstOrDefault().Id : 0))
         .ForMember(dest => dest.ApplicationId, src => src.MapFrom(vp => vp.Version.ApplicationId))
              .ForMember(dest => dest.SubscriptionType, src => src.MapFrom(vp => vp.Version.Application.SubscriptionType))
         .ForMember(dest => dest.Price, src => src.MapFrom<ApplicationPriceVersionMinimumPriceResolver, int>(v => v.VersionId));



            #endregion

            #region Application Features

            CreateMap<CreateVersionFeatureAPIInputDto, CreateVersionFeatureInputDto>()
                .ConvertUsing<CreateVersionFeatureTypeConverter>();

            CreateMap<UpdateVersionFeatureAPIInputDto, UpdateVersionFeatureInputDto>()
               .ConvertUsing<UpdateVersionFeatureTypeConverter>();

            CreateMap<List<VersionFeature>, GetVersionFeatureOutputDto>()
              .ConvertUsing<GetVersionFeatureOutputListToEntityTypeConverter>();

            CreateMap<VersionFeature, GetVersionFeatureOutputDto>()
          .ConvertUsing<GetVersionFeatureOutputTypeConverter>();

            CreateMap<Feature, GetAllVersionFeatureOutputDto>()
               .ForMember(dest => dest.FeatureName
               , opts => opts.MapFrom(s => s.Name))
                ;
            //from view
            //CreateMap<ViewApplicationVersionFeature, GetAssignedFeatureVersionsDto>();
            //CreateMap<List<ViewApplicationVersionFeature>, GetVersionFeatureOutputDto>()
            //    .ConvertUsing<GetVersionFeatureViewOutputViewTypeConverter>();
            //CreateMap<ViewApplicationVersionFeature, GetVersionFeatureOutputDto>();



            CreateMap<VersionFeature, GetAssignedFeatureVersionsDto>()
               .ForMember(dest => dest.FeatureName
               , opts => opts.MapFrom(s => s.Feature.Name))
               .ForMember(dest => dest.VersionName
               , opts => opts.MapFrom(s => s.Version.Name))
               .ForMember(dest => dest.ApplicationName
               , opts => opts.MapFrom(s => s.Version.Application.Name));

            CreateMap<CreateVersionFeatureInputDto, GetVersionFeatureOutputDto>()
             .ForMember(dest => dest.Versions
            , opts => opts.Ignore());

            CreateMap<Version, GetVersionAssignFeatureOutputDto>()
                 .ForMember(dest => dest.VersionName
               , opts => opts.MapFrom(s => s.Name));
            #endregion

            #region Feature
            CreateMap<Feature, GetUnAsignedFeatureOutputDto>()
                .ForMember(x => x.FeatureName, opt =>
                   opt.MapFrom(s => s.Name));
            #endregion

            #region AddOn
            CreateMap<AddOn, GetUnAsignedAddOnOutputDto>()
               .ForMember(x => x.AddOnName, opt =>
                  opt.MapFrom(s => s.Name));
            #endregion

            #region Module
            CreateMap<Module, GetUnAsignedModuleOutputDto>()
               .ForMember(x => x.ModuleName, opt =>
                  opt.MapFrom(s => s.Name));
            #endregion

            #region Application Module

            CreateMap<CreateVersionModuleAPIInputDto, CreateVersionModuleInputDto>()
                .ConvertUsing<CreateVersionModuleTypeConverter>();

            CreateMap<UpdateVersionModuleAPIInputDto, UpdateVersionModuleInputDto>()
               .ConvertUsing<UpdateVersionModuleTypeConverter>();

            CreateMap<List<VersionModule>, GetVersionModuleOutputDto>()
              .ConvertUsing<GetVersionModuleOutputListToEntityTypeConverter>();

            CreateMap<VersionModule, GetVersionModuleOutputDto>()
          .ConvertUsing<GetVersionModuleOutputTypeConverter>();




            CreateMap<VersionModule, GetAssignedModuleVersionsDto>()
               .ForMember(dest => dest.ModuleName
               , opts => opts.MapFrom(s => s.Module.Name))
               .ForMember(dest => dest.VersionName
               , opts => opts.MapFrom(s => s.Version.Name))
               .ForMember(dest => dest.ApplicationName
               , opts => opts.MapFrom(s => s.Version.Application.Name));

            CreateMap<CreateVersionModuleInputDto, GetVersionModuleOutputDto>()
             .ForMember(dest => dest.Versions
            , opts => opts.Ignore());


            #endregion

            #region Application AddOn

            CreateMap<CreateVersionAddOnAPIInputDto, CreateVersionAddOnInputDto>()
                .ConvertUsing<CreateVersionAddOnTypeConverter>();

            CreateMap<UpdateVersionAddOnAPIInputDto, UpdateVersionAddOnInputDto>()
               .ConvertUsing<UpdateVersionAddOnTypeConverter>();

            CreateMap<List<VersionAddon>, GetVersionAddOnOutputDto>()
              .ConvertUsing<GetVersionAddOnOutputListToEntityTypeConverter>();

            CreateMap<VersionAddon, GetVersionAddOnOutputDto>()
          .ConvertUsing<GetVersionAddOnOutputTypeConverter>();



            CreateMap<VersionAddon, GetAssignedAddOnVersionsDto>()
               .ForMember(dest => dest.AddOnName
               , opts => opts.MapFrom(s => s.Addon.Name))
               .ForMember(dest => dest.VersionName
               , opts => opts.MapFrom(s => s.Version.Name))
               .ForMember(dest => dest.ApplicationName
               , opts => opts.MapFrom(s => s.Version.Application.Name));

            CreateMap<CreateVersionAddOnInputDto, GetVersionAddOnOutputDto>()
             .ForMember(dest => dest.Versions
            , opts => opts.Ignore());


            #endregion

            #region Tax
            CreateMap<Tax, GetTaxOutputDto>()
              .ForMember(dest => dest.CountryName, opts => opts.MapFrom(s => s.Country.Name))
              .ForMember(dest => dest.CreateDate, opts => opts.MapFrom(s => DateTime.SpecifyKind(s.CreateDate, DateTimeKind.Utc)))
              .ForMember(dest => dest.CreatedBy, opts => opts.MapFrom(s => s.CreatedByNavigation.Name));

            #endregion

            #region Customers

            #region Download Center
            CreateMap<Application, GetDownloadCenterApplicationsOutputDto>()
               .ForMember(dest => dest.Logo
                   , opts => opts.MapFrom(s => s.Image))

               .ForMember(dest => dest.SubscriptionType
                   , opts => opts.MapFrom(s => s.SubscriptionType.Name))
               .ForMember(dest => dest.Versions
                   , opts => opts.MapFrom(s => s.Versions))
               .ForMember(dest => dest.Rate, src => src.MapFrom<ApplicationReviewResolver, int>(a => a.Id));

            CreateMap<Version, GetDownloadCenterApplicationVersionOutputDto>()
                 .ForMember(dest => dest.Logo
                   , opts => opts.MapFrom(s => s.Image))
              .ForMember(dest => dest.VersionRelease
               , opts => opts.MapFrom(s => s.VersionReleases.FirstOrDefault(x => x.IsCurrent)))
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;

            #endregion

            #region Browse Apps & Filter
            CreateMap<Application, BrowseAppsApplicationOutputDto>()
              .ForMember(dest => dest.Image
                  , opts => opts.MapFrom(s => s.Image))
              .ForMember(dest => dest.SubscriptionType
                  , opts => opts.MapFrom(s => s.SubscriptionType.Name))
              .ForMember(dest => dest.Label
                  , opts => opts.MapFrom(s => s.ApplicationLabels.FirstOrDefault()))
                  .ForMember(dest => dest.Description
                  , opts => opts.MapFrom(s => s.ShortDescription))
             //.ForMember(dest => dest.FeaturedTag
             //     , opts => opts.MapFrom(s => s.ApplicationTags.Any(x => x.IsFeatured) && s.ApplicationTags.FirstOrDefault(x => x.IsFeatured).Tag != null ?  s.ApplicationTags.Where(x => x.IsFeatured).FirstOrDefault().Tag.Name : String.Empty))
             .ForMember(dest => dest.ApplicationTags
                  , opts => opts.MapFrom(s => s.ApplicationTags))
            .ForMember(dest => dest.Price, src => src.MapFrom<ApplicationMinimumPriceResolver, int>(a => a.Id))
            .ForMember(dest => dest.PriceDetails, src => src.MapFrom<ApplicationMinimumPricesResolver, int>(a => a.Id))
            .ForMember(dest => dest.Rate, src => src.MapFrom<ApplicationReviewResolver, int>(a => a.Id))
            ;

            CreateMap<AddOn, BrowseAppsAddOnOutputDto>()
                      .ForMember(dest => dest.Image
                          , opts => opts.MapFrom(s => s.Logo))
                        .ForMember(dest => dest.Name
                          , opts => opts.MapFrom(s => s.Name))
                      .ForMember(dest => dest.ApplicationId
                          , opts => opts.MapFrom(s => s.VersionAddons.Where(x => x.Version != null && x.Version.ApplicationId > 0).Select(va => va.Version.ApplicationId).Distinct()))
                                     .ForMember(dest => dest.Label
                   , opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
                          .ForMember(dest => dest.Description
                          , opts => opts.MapFrom(s => s.ShortDescription))
                         .ForMember(dest => dest.PriceDetails, src => src.MapFrom<AddonMinimumPriceResolver_, int>(a => a.Id))
                         .ForMember(dest => dest.AddOnTags
                          , opts => opts.MapFrom(s => s.AddOnTags))
                        .ForMember(dest => dest.Price, src => src.MapFrom<AddonMinimumPriceResolver, int>(a => a.Id));



            #endregion

            #region Wishlist
            //from wishlist to output
            CreateMap<WishListApplication, GetWishlistApplicationOutputDto>();
            // .ForMember(dest=>dest.Application.SubscribtionTypeId,opts=>opts.)
            //.ForPath(dest => dest.Application.SubscribtionTypeId, opts => opts.MapFrom(src => src.Application.SubscriptionTypeId))
            //.ForPath(dest => dest.Application.Name, opts => opts.MapFrom(src => src.Application.Name))
            //.ForPath(dest => dest.Application.ShortDescription, opts => opts.MapFrom(src => src.Application.ShortDescription))
            //.ForPath(dest => dest.Application.Title, opts => opts.MapFrom(src => src.Application.Title))
            //.ForPath(dest => dest.Application.Logo, opts => opts.MapFrom(src => src.Application.Image))
            //.ForPath(dest => dest.Application.Label, opts => opts.MapFrom(src => src.Application.ApplicationLabels))
            //.ForPath(dest => dest.Application.FeaturedTag, opts => opts.MapFrom(src => src.Application.ApplicationTags));


            CreateMap<WishListAddOn, GetWihlistAddOnOutputDto>();

            CreateMap<Application, GetApplicationWishlistOutputDto>()
                              .ForMember(dest => dest.SubscribtionTypeId, opts => opts.MapFrom(s => s.SubscriptionTypeId))

              .ForMember(dest => dest.Label, opts => opts.MapFrom(s => s.ApplicationLabels.FirstOrDefault()))
              .ForMember(dest => dest.FeaturedTag, opts => opts.MapFrom(s => s.ApplicationTags.FirstOrDefault(t => t.IsFeatured) != null ? s.ApplicationTags.FirstOrDefault(t => t.IsFeatured).Tag.Name : string.Empty))
              .ForMember(dest => dest.Logo, opts => opts.MapFrom(s => s.Image))
              .ForMember(dest => dest.Price, opts => opts.MapFrom<ApplicationMinimumPriceResolver, int>(src => src.Id))
              .ForMember(dest => dest.Rate, src => src.Ignore())
            ;


            CreateMap<AddOn, GetAddOnWishlistOutputDto>()
               .ForMember(dest => dest.Label, opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
               .ForMember(dest => dest.Price, opts => opts.MapFrom<AddonMinimumPriceResolver, int>(src => src.Id));
            ;

            #endregion

            #region Review
            CreateMap<CustomerReview, GetCustomerReviewOutputDto>()
                 .ForMember(dest => dest.ApplicationName, opts => opts.MapFrom(s => s.Application.Name))
                 .ForMember(dest => dest.CustomerName, opts => opts.MapFrom(s => s.Customer.Name))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(s => s.Status.Status));

            CreateMap<CustomerReview, CustomerReviewDto>()
                .ForMember(dest => dest.Status, src => src.MapFrom(r => r.Status.Status));

            CreateMap<CustomerReview, ReviewDto>()
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(r => r.Customer.Name));

            #endregion

            #region CustomerHistoryRegistred

            CreateMap<Customer, GetCustomerRegistrationHistoryOutputDto>()
                //.ForMember(dest => dest.Versions, opt => opt.MapFrom(s => s.CustomerSubscriptions.Where(x => !x.IsAddOn).SelectMany(x => x.VersionSubscriptions).Select(v => v.VersionName).ToList()))
                .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.CustomerEmail, opt =>
                opt.MapFrom(s => s.CustomerEmails.Where(x => x.IsPrimary == true).FirstOrDefault()))
                .ForMember(dest => dest.CustomerMobile, opt =>
                opt.MapFrom(s => s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault()))
                .ForMember(dest => dest.CustomerSubscriptions, opt => opt.MapFrom(s => s.CustomerSubscriptions))
                .ForMember(dest => dest.Country, opt =>
                opt.MapFrom(s => s.Country.Name))
                .ForMember(dest => dest.RegistrationDate, opt =>
                opt.MapFrom(s => DateTime.SpecifyKind(s.CreateDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(s => s.CustomerStatus.Status))
                .ForMember(dest => dest.StatusId, opt =>
                opt.MapFrom(s => s.CustomerStatusId))
                .ForMember(dest => dest.Versions, opt =>
                opt.MapFrom(s => s.DownloadVersionLogs.Select(x => x.VersionIdRelease.Version)));
            CreateMap<CustomerEmail, CustomerEmailDto>();
            CreateMap<CustomerMobile, CustomerMobileDto>();
            CreateMap<Version, VersionTitle>();

            #endregion


            #region Account
            CreateMap<Customer, GetCustomerOutputDto>()
                .ForMember(dest => dest.CompanySize, opt => opt.MapFrom(s => s.CompanySize))
                 .ForMember(dest => dest.Logo, opts =>
                   opts.MapFrom<BlobURLResolver, FileStorage>(src => src.Image))
                .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(s => s.CustomerEmails.Any() ? s.CustomerEmails.Where(x => x.IsPrimary == true).FirstOrDefault().Email : null))
                .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(s => s.CustomerMobiles.Any() ? s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault().Mobile : null))
                .ForMember(dest => dest.PhoneCode, opt =>
                    opt.MapFrom(s => s.CustomerMobiles.Any() ? s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault().PhoneCode : null))
                .ForMember(dest => dest.Country, opt =>
                opt.MapFrom(s => s.Country.Name))
                .ForMember(dest => dest.Industry, opt =>
                opt.MapFrom(s => s.Industry))
                 .ForMember(dest => dest.CustomerCards, opt => opt.MapFrom(s => s.CustomerCards));
            CreateMap<Customer, GetSimplifiedCustomerOutputDto>()
                .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(s => s.CustomerEmails.Any() ? s.CustomerEmails.Where(x => x.IsPrimary == true).FirstOrDefault().Email : null))
                .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(s => s.CustomerMobiles.Any() ? s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault().Mobile : null));

            CreateMap<Customer, GetCustomerImageOutputDto>()
              .ForMember(dest => dest.Logo, opts =>
                   opts.MapFrom<BlobURLResolver, FileStorage>(src => src.Image));


            CreateMap<Industry, IndustryOutputDto>();
            CreateMap<CompanySize, CompanySizeOutputDto>();

            #endregion

            #region CustomerAppTagsOrHiglighted
            CreateMap<Application, GetCustomerAppTagsOrHighlightedOutputDto>()
                .ForMember(dest => dest.Logo, src =>
                src.MapFrom(a => a.Image))
                .ForMember(dest => dest.Price, src =>
                src.MapFrom<ApplicationMinimumPriceResolver, int>(a => a.Id))
                ;
            #endregion

            #region Contract

            #region Customer
            CreateMap<Country, GetAllCountriesOutputDto>()
                  .ForMember(dest => dest.Country
            , opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Currency
            , opt => opt.Ignore());
            CreateMap<VersionSubscription, VersionTitle>()
               .ForMember(dest => dest.Name
            , opt => opt.MapFrom(s => s.VersionName))
               .ForMember(dest => dest.Title
            , opt => opt.MapFrom(s => s.VersionPrice.Version.Title));
            CreateMap<CustomerSubscription, CustomerSubscriptionDto>()
                .ForMember(dest => dest.Versions, opt =>
                opt.MapFrom(s => s.VersionSubscriptions));

            CreateMap<Customer, GetContractCustomerOutputDto>()
                .ForMember(dest => dest.IsActive, opt =>
                opt.MapFrom(s => s.CustomerStatusId != (int)CustomerStatusEnum.Suspended ? true : false))
                .ForMember(dest => dest.Invoiced, opt =>
                opt.MapFrom(s => s.CustomerSubscriptions.Any(cs => cs.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)) && (s.CustomerStatusId == (int)CustomerStatusEnum.Registered ||
                s.CustomerStatusId == (int)CustomerStatusEnum.Suspended) ? true : false))
                .ForMember(dest => dest.ContractId, opt =>
                opt.MapFrom(s => s.Contract.Serial))
                .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(s => s.CustomerEmails.Where(x => x.IsPrimary == true).FirstOrDefault().Email))
                .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(s => s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault().Mobile))
                .ForMember(dest => dest.Country, opt =>
                opt.MapFrom(s => s.Country.Name))
                //.ForMember(dest => dest.Currency, opts =>
                //     opts.MapFrom<DefaultCurrencyResolver, string>(src => src.Country.CountryCurrency.Currency.Code))

                .ForMember(dest => dest.Currency, opt =>
                opt.MapFrom(s => s.Country.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.RegistrationDate, opt =>
                opt.MapFrom(s => DateTime.SpecifyKind(s.CreateDate, DateTimeKind.Utc)))
               //.ForMember(dest => dest.CustomerSubscriptions, opt => opt.MapFrom(s => s.CustomerSubscriptions))
               .ForMember(dest => dest.Versions, opt =>
                opt.MapFrom(s => s.DownloadVersionLogs.Select(x => x.VersionIdRelease.Version)));
            ;

            CreateMap<Customer, GetCustomerReferencesOutputDto>()
               .ForMember(dest => dest.IsActive, opt =>
               opt.MapFrom(s => s.CustomerStatusId == (int)CustomerStatusEnum.Registered ? true : false))
                .ForMember(dest => dest.ContractId, opt =>
               opt.MapFrom(s => s.Contract.Serial))
               .ForMember(dest => dest.Email, opt =>
               opt.MapFrom(s => s.CustomerEmails.Where(x => x.IsPrimary == true).FirstOrDefault().Email))
               .ForMember(dest => dest.Phone, opt =>
               opt.MapFrom(s => s.CustomerMobiles.Where(x => x.IsPrimary == true).FirstOrDefault().Mobile))
               .ForMember(dest => dest.Country, opt =>
               opt.MapFrom(s => s.Country.Name))
               .ForMember(dest => dest.CountryId, opt =>
               opt.MapFrom(s => s.Country.Id))
               .ForMember(dest => dest.CurrencyName, opt =>
               opt.MapFrom(s => s.Country.CountryCurrency.Currency.Name))
               .ForMember(dest => dest.CompanySize, opt => opt.MapFrom(s => s.CompanySize.Size))
               .ForMember(dest => dest.Logo, opts =>
                   opts.MapFrom<AdminCustomCutomerURLResolver, FileStorage>(src => src.Image))
               .ForMember(dest => dest.Industry, opt =>
               opt.MapFrom(s => s.Industry.Name))
               .ForMember(dest => dest.CustomerApplications, opt =>
               opt.Ignore())
               .ForMember(dest => dest.Status, opt =>
               opt.MapFrom(s => s.CustomerStatus.Status))
                ;

            CreateMap<Version, CustomerApplicationDto>()
                .ForMember(dest => dest.Logo, src =>
                src.MapFrom(a => a.Image))
                .ForMember(dest => dest.Price, src =>
                src.MapFrom<ApplicationMinimumPriceResolver, int>(a => a.ApplicationId))
                ;

            CreateMap<License, CustomerReguesttDto>()
                .ForMember(dest => dest.Status, opt =>
                   opt.MapFrom(l => l.LicenseStatus.Status));

            //Todo After CustomerActivites in db
            CreateMap<Customer, GetCustomerActivitesOutputDto>();
            #endregion

            #region Licences
            CreateMap<License, GetAllLicensesOutputDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(l => l.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(l => l.CustomerSubscription.Customer.Contract.Serial))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(l => l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName))
                .ForMember(dest => dest.OldDevice, opt => opt.MapFrom(l => l.DeviceName))
                .ForMember(dest => dest.OldSerial, opt => opt.MapFrom(l => l.Serial))
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(l => l.CreateDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(l => l.LicenseStatus.Status));

            CreateMap<RequestChangeDevice, RequestChangeDevicesOutputDto>()
                 .ForMember(dest => dest.LicencesId, opt => opt.MapFrom(r => r.LicenseId))
                 .ForMember(dest => dest.NewDevice, opt => opt.MapFrom(r => r.NewDeviceName))
                 .ForMember(dest => dest.OldDevice, opt => opt.MapFrom(r => r.OldDeviceName))
                 .ForMember(dest => dest.NewSerial, opt => opt.MapFrom(r => r.NewSerial))
                 .ForMember(dest => dest.OldSerial, opt => opt.MapFrom(r => r.OldSerial))
                 .ForMember(dest => dest.Reason, opt => opt.MapFrom(r => r.ReasonChangeDevice.Reason));

            CreateMap<ReasonChangeDevice, ReasonChangeDeviceOutputDto>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(s => s.Reason));
            #endregion

            #region Invoices
            CreateMap<Invoice, GetInvoicesOutputDto>()
                .ForMember(dest => dest.ContractId, opt =>
                opt.MapFrom(i => i.CustomerSubscription.Customer.Contract.Serial))
                .ForMember(dest => dest.Customer, opt =>
                opt.MapFrom(i => i.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.Version, src =>
                src.MapFrom(i => i.InvoiceDetails.FirstOrDefault().VersionName))
               .ForMember(dest => dest.Currency, src =>
                src.MapFrom(i => i.CustomerSubscription.CurrencyName))
               .ForMember(dest => dest.PaymentMethd, src =>
                src.MapFrom(i => i.PaymentMethod.PaymentType.Name))
               .ForMember(dest => dest.InvoiceStatus, src =>
                src.MapFrom(i => i.InvoiceStatus.Status))
                ;

            #endregion

            #endregion

            #region CustomerProduct
            CreateMap<Version, GetCustomerApplicationVersionsDto>()
               .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.Image))
               .ForMember(dest => dest.SubscriptionType, src => src.MapFrom(a => a.Application.SubscriptionTypeId))
                .ForMember(dest => dest.MinVersionPrice, src => src.MapFrom<VersionMinimumPriceResolver, int>(v => v.Id))

            ;





            CreateMap<Invoice, GetCustomerSubscriptionOutputDto>()
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(a => a.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.AutoBill, src => src.MapFrom(a => a.CustomerSubscription.AutoBill))
                 .ForMember(dest => dest.StartDate, src => src.MapFrom(i => i.CustomerSubscription.CreateDate))
                 ;
            CreateMap<VersionSubscription, GetCustomerProductOutputDto>()
                  //.ForMember(dest => dest.Price, src => src.MapFrom<ApplicationMinimumPriceResolver, int>(a => a.VersionPrice.Version.ApplicationId))
                  .ForMember(dest => dest.Price, src => src.Ignore())
                .ForMember(dest => dest.VersionSubscriptionId, opt => opt.MapFrom(s => s.Id))
                .ForMember(dest => dest.CustomerSubscriptionId, opt => opt.MapFrom(s => s.CustomerSubscriptionId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(s => s.VersionRelease.VersionId))
                .ForMember(dest => dest.ApplicationName, opt => opt.MapFrom(s => s.VersionRelease.Version.Application.Name))
                .ForMember(dest => dest.VersionReleaseId, opt => opt.MapFrom(s => s.VersionReleaseId))
                .ForMember(dest => dest.Logo, src => src.MapFrom(a => a.VersionRelease.Version.Image))
                .ForMember(dest => dest.PriceLevel, opt => opt.MapFrom(s => s.VersionPrice.PriceLevel.Name))
                .ForMember(dest => dest.PriceLevelId, opt => opt.MapFrom(s => s.VersionPrice.PriceLevelId))
                 ////.ForMember(dest => dest.Currency, opt => opt.MapFrom(s => s.CustomerSubscription.CurrencyName))
                 .ForMember(dest => dest.Currency, opt => opt.MapFrom(s => s.CustomerSubscription.CurrencyName))

                //.ForMember(dest => dest.Currency, opts =>
                // opts.MapFrom<DefaultCurrencyResolver, string>(src => src.CustomerSubscription.CurrencyName))
                .ForMember(dest => dest.ReleaseNumber, opt => opt.MapFrom(s => s.VersionRelease.ReleaseNumber))
                .ForMember(dest => dest.UsedDevice, opt => opt.MapFrom<VersionSubscriptionUsedDeviceResolver, int>(a => a.Id))
                .ForMember(dest => dest.NumberOfLicenses, opt => opt.MapFrom(s => s.CustomerSubscription.NumberOfLicenses))
                .ForMember(dest => dest.RenewDate, src => src.MapFrom<GetRenewalDateInvoiceResolver, int>(a => a.CustomerSubscriptionId)
                );

            CreateMap<VersionSubscription, CustomerProductWorkspacesDto>()
                .ForMember(dest => dest.UsedLicesesCount, opt => opt.MapFrom(v => v.CustomerSubscription.Licenses.Count))
                .ForMember(dest => dest.LicensesCount, opt => opt.MapFrom(v => v.CustomerSubscription.NumberOfLicenses))
                .ForMember(dest => dest.NextRenewalDate, opt => opt.MapFrom(v => v.CustomerSubscription.Invoices.Where(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Paid).OrderByDescending(i => i.CreateDate).FirstOrDefault().EndDate));

            CreateMap<VersionSubscription, CustomerProductLookupDto>()
                .ForMember(dest => dest.VersionPriceId, opt => opt.MapFrom(v => v.VersionPriceId))
                .ForMember(dest => dest.CanCreate, opt => opt.MapFrom(v => v.CustomerSubscription.NumberOfLicenses > v.CustomerSubscription.Licenses.Count))
                .ForMember(dest => dest.PaiedStatus, opt => opt.MapFrom(v => !v.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid)))
                .ForMember(dest => dest.CrmId, opt => opt.MapFrom(v => v.VersionPrice != null && v.VersionPrice.Version != null ? v.VersionPrice.Version.ProductCrmId : string.Empty))
                         .ForMember(dest => dest.VersionSubscriptionId, opt => opt.MapFrom(v => v.Id));

            CreateMap<VersionSubscription, GetCustomerProductLicensesOutputDto>()
                .ForMember(dest => dest.Licences
                , opt => opt.MapFrom(s => s.CustomerSubscription.Licenses));

            CreateMap<VersionRelease, GetReleasesOutputDto>()
             .ForMember(dest => dest.VersionName
             , opt => opt.MapFrom(s => s.Version.Name))
             .ForMember(dest => dest.VersionId
             , opt => opt.MapFrom(s => s.VersionId))
             //.ForMember(dest => dest.VersionDate
             //, opt => opt.MapFrom(s => s.VersionPrice.Version.CreateDate))
             .ForMember(dest => dest.ReleaseTitle
             , opt => opt.MapFrom(s => s.ReleaseNumber))
              .ForMember(dest => dest.DownloadUrl
             , opt => opt.MapFrom(s => s.DownloadUrl))
               .ForMember(dest => dest.Description
             , opt => opt.MapFrom(s => s.Version.ShortDescription))
              .ForMember(dest => dest.Released
             , opt => opt.MapFrom(s => s.CreateDate))

             ;

            CreateMap<License, GetLicenseOutputDto>()
                .ForMember(dest => dest.ActivatedOn, opt => opt.MapFrom(s => s.ActivateOn ?? null))
                .ForMember(dest => dest.CanCreate, opt => opt.MapFrom(s => s.CustomerSubscription.NumberOfLicenses > s.CustomerSubscription.Licenses.Count))
                .ForMember(dest => dest.PaiedStatus, opt => opt.MapFrom(v => !v.CustomerSubscription.Invoices.Any(i => i.InvoiceStatusId == (int)InvoiceStatusEnum.Unpaid)))
                .ForMember(dest => dest.RenwalDate, opt => opt.MapFrom(s => s.RenewalDate))
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(s => s.Serial))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(s => s.LicenseStatusId))
                .ForMember(dest => dest.CustomerSubscriptionId, opt => opt.MapFrom(s => s.CustomerSubscriptionId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(s => s.LicenseStatus.Status))
                .ForMember(dest => dest.HasChangedRequest, opt => opt.MapFrom(s => s.RequestChangeDevices.Any(s => s.RequestChangeDeviceStatusId == (int)RequestChangeDeviceStatusEnum.InProgress)))
                .ForMember(dest => dest.HasRelatedAddonLicense,
                                   opt => opt.MapFrom(s => !
                                   (!s.CustomerSubscription.VersionSubscriptions.FirstOrDefault().AddonSubscriptions.Any() ||
                                   !s.CustomerSubscription.VersionSubscriptions.FirstOrDefault().AddonSubscriptions
                                        .Any(a => a.CustomerSubscription.Licenses.Any(l => l.Serial.ToLower().Equals(s.Serial.ToLower()))))))
                .ForMember(dest => dest.File, opt => opt.MapFrom<BlobURLResolver, FileStorage>(s => s.ActivationFile));



            CreateMap<License, GetCustomerProductLicenseAddonOutputDto>()
                .ForMember(dest => dest.SerialNumber
                , opt => opt.MapFrom(s => s.Serial));
            CreateMap<RequestActivationKey, DownloadFileActivationOutputDto>();
            CreateMap<ReasonChangeDevice, GetReasonChangeDeviceOutputDto>();
            CreateMap<VersionSubscription, GetAllCustomerProductAddonOutputDto>();
            CreateMap<AddonSubscription, GetAllCustomerProductAddonOutputDto>()
                .ForMember(dest => dest.AddOnId, opt => opt.MapFrom(e => e.Id))
                .ForMember(dest => dest.AddonSubscriptionId, opt => opt.MapFrom(e => e.CustomerSubscriptionId))
                .ForMember(dest => dest.PriceLevelId, opt => opt.MapFrom(e => e.AddonPrice.PriceLevelId))
                .ForMember(dest => dest.AddOnName, opt => opt.MapFrom(e => e.AddonName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(e => e.AddonPrice.AddOn.ShortDescription))
                // .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(e => e.CreateDate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(e => e.AddonPrice.AddOn.IsActive))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(e => e.AddonPrice.AddOn.Logo))
                .ForMember(dest => dest.IsPurshased, opt => opt.MapFrom(e => true))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(e => e.AddonPrice))
                .ForMember(dest => dest.Label, opts => opts.MapFrom(s => s.AddonPrice.AddOn.AddOnLabels.FirstOrDefault()))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(s => s.AddonPrice.AddOn.AddOnTags.Where(x => x.IsFeatured && x.Tag != null).Select(t => t.Tag).FirstOrDefault()))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(s => s.VersionSubscriptionId))

                ;

            CreateMap<AddOn, GetAllCustomerProductAddonOutputDto>()
                .ForMember(dest => dest.AddOnName
                , opt => opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.Description
                , opt => opt.MapFrom(s => s.ShortDescription))
                .ForMember(dest => dest.Image
                , opt => opt.MapFrom(s => s.Logo))
                .ForMember(dest => dest.AddOnId
                , opt => opt.MapFrom(s => s.Id))

                          .ForMember(dest => dest.Tag
                , opt => opt.MapFrom(s => s.AddOnTags.Where(x => x.IsFeatured && x.Tag != null).Select(t => t.Tag).FirstOrDefault()))

                    .ForMember(dest => dest.Label
                   , opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
                 .ForMember(dest => dest.AddonSubscriptionId
                , opt => opt.Ignore())
                  .ForMember(dest => dest.PurshasedData
                , opt => opt.Ignore())
                   .ForMember(dest => dest.Price
                , opt => opt.Ignore())
                        .ForMember(dest => dest.AllPrices
                , opt => opt.Ignore())
                  // .ForMember(dest => dest.Price
                  //, opt => opt.MapFrom<AddonMinimumPriceResolver, int>(a => a.Id))
                  .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ;
            CreateMap<AddonPriceDetailsDto, MonthlyPriceDetailsDto>().ReverseMap();
            CreateMap<AddonPriceDetailsDto, YearlyPriceDetailsDto>().ReverseMap();
            CreateMap<AddonPriceDetailsDto, ForeverPriceDetailsDto>().ReverseMap();

            CreateMap<AddonSubscription, ProductAddonDetailsDto>()
                .ForMember(dest => dest.LicenseId, src => src.MapFrom(a => a.CustomerSubscription.Licenses.Any() ? a.CustomerSubscription.Licenses.FirstOrDefault().Id : default))
                .ForMember(dest => dest.AddonTitle, src => src.MapFrom(a => a.AddonName))
                .ForMember(dest => dest.PurchasedDate, src => src.MapFrom(a => a.CustomerSubscription.CreateDate))
                .ForMember(dest => dest.RenewalDate, src => src.MapFrom(a => a.CustomerSubscription.Invoices.OrderBy(i => i.CreateDate).LastOrDefault().EndDate))
                .ForMember(dest => dest.LicenseStatusId, src => src.MapFrom(a => a.CustomerSubscription.Licenses.Any() ? a.CustomerSubscription.Licenses.FirstOrDefault().LicenseStatusId : default))
                .ForMember(dest => dest.File, src => src.MapFrom(a => a.CustomerSubscription.Licenses.Any() ? a.CustomerSubscription.Licenses.FirstOrDefault().ActivationFile : null));

            CreateMap<AddonSubscription, GetAddOnPurshasedOutputDto>()
                //.ForMember(dest=>dest.Status,opt=>opt.MapFrom(s=>s.CustomerSubscription.Licenses))
                .ForMember(dest => dest.RenwalEvery
                , opt => opt.MapFrom(s => s.CustomerSubscription.RenewEvery))
                .ForMember(dest => dest.Status
                , opt => opt.MapFrom(s => s.CustomerSubscription.Licenses.FirstOrDefault().RenewalDate));
            CreateMap<AddOn, GetAddOnPriceNotPurshased>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom<AddonMinimumPriceResolver, int>(a => a.Id));
            CreateMap<AddOnPrice, AddOnPriceDto>();


            CreateMap<Application, CustomerApplicationLookupDto>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(s => s.Id))

                .ForMember(dest => dest.Name, opt => opt.MapFrom(s => s.Name));
            #endregion

            #region Licences
            CreateMap<Customer, LicenseCustomerLookupDto>()
                .ForMember(dest => dest.ContractSerial, opt => opt.MapFrom(s => s.Contract.Serial));

            CreateMap<CustomerSubscription, LicenseProductLookupDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(s => s.VersionSubscriptions.FirstOrDefault().VersionName))
                .ForMember(dest => dest.UsedLicenseCount, src => src.MapFrom(s => s.Licenses.Count))
                .ForMember(dest => dest.TotalLicenseCount, src => src.MapFrom(s => s.NumberOfLicenses))
                .ForMember(dest => dest.LastInvoiceStartDate, src => src.MapFrom(s => s.Invoices.Any() ? s.Invoices.Last().StartDate : default))
                .ForMember(dest => dest.LastInvoiceEndDate, src => src.MapFrom(s => s.Invoices.Any() ? s.Invoices.Last().EndDate : default))
                .ForMember(dest => dest.Image, src => src.MapFrom(s => s.VersionSubscriptions.FirstOrDefault().VersionPrice.Version.Image));

            CreateMap<RequestChangeDevice, GetRequestChangeDeviceOutputDto>()
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(s => s.License.ActivateOn))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(s => s.RequestChangeDeviceStatus.Status))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(s => s.ReasonChangeDevice.Reason));

            CreateMap<License, ExpiredLicenseDto>()
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(l => l.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.RenewalDate, src => src.MapFrom(l => l.RenewalDate.HasValue ? DateTime.SpecifyKind(l.RenewalDate.Value, DateTimeKind.Utc) : l.RenewalDate))
                .ForMember(dest => dest.ActivateOn, src => src.MapFrom(l => l.ActivateOn.HasValue ? DateTime.SpecifyKind(l.ActivateOn.Value, DateTimeKind.Utc) : l.ActivateOn))
                .ForMember(dest => dest.ContractSerial, src => src.MapFrom(l => l.CustomerSubscription.Customer.Contract.Serial))
                .ForMember(dest => dest.ProductName, src => src.MapFrom(l => l.CustomerSubscription.IsAddOn ?
                                                                             l.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName :
                                                                             l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName));

            CreateMap<License, ActiveLicenseDto>()
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(l => l.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.ContractSerial, src => src.MapFrom(l => l.CustomerSubscription.Customer.Contract.Serial))
                .ForMember(dest => dest.ProductName, src => src.MapFrom(l => l.CustomerSubscription.IsAddOn ?
                                                                             l.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName :
                                                                             l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName))
                .ForMember(dest => dest.LicenseFile, src => src.MapFrom<BlobURLResolver, FileStorage>(l => l.ActivationFile));

            CreateMap<RequestChangeDevice, ChangeLicenseDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(r => r.License.Id))
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(r => r.License.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.ContractSerial, src => src.MapFrom(r => r.License.CustomerSubscription.Customer.Contract.Serial))
                .ForMember(dest => dest.ProductName, src => src.MapFrom(r => r.License.CustomerSubscription.IsAddOn ?
                                                                             r.License.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName :
                                                                             r.License.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName))
                .ForMember(dest => dest.ActivateOn, src => src.MapFrom(r => r.License.ActivateOn.HasValue ? DateTime.SpecifyKind(r.License.ActivateOn.Value, DateTimeKind.Utc) : r.License.ActivateOn))
                .ForMember(dest => dest.OldSerialNumber, src => src.MapFrom(r => r.OldSerial))
                .ForMember(dest => dest.Serial, src => src.MapFrom(r => r.NewSerial))
                .ForMember(dest => dest.RenewalDate, src => src.MapFrom(r => r.License.RenewalDate.HasValue ? DateTime.SpecifyKind(r.License.RenewalDate.Value, DateTimeKind.Utc) : r.License.RenewalDate))
                .ForMember(dest => dest.DeviceName, src => src.MapFrom(r => r.License.DeviceName))
                .ForMember(dest => dest.ChangedDate, src => src.MapFrom(r => r.ModifiedDate.HasValue ? DateTime.SpecifyKind(r.ModifiedDate.Value, DateTimeKind.Utc) : r.ModifiedDate));

            CreateMap<ViewLicenseRequest, LicenseRequestDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(r => r.LicenseId))
                .ForMember(dest => dest.CreateDate, src => src.MapFrom(r => DateTime.SpecifyKind(r.CreateDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.ProductName, src => src.MapFrom(r => r.IsAddOn ? r.AddonName : r.VersionName));

            CreateMap<License, LicenseLogBaseInfoDto>()
                .ForMember(dest => dest.CustomerName, src => src.MapFrom(l => l.CustomerSubscription.Customer.Name))
                .ForMember(dest => dest.Product, src => src.MapFrom(l => l.CustomerSubscription.IsAddOn ?
                                                                         l.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonName :
                                                                         l.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionName));

            CreateMap<LicenseLog, LicenseLogDto>()
                .ForMember(dest => dest.Date, src => src.MapFrom(l => l.CreateDate))
                .ForMember(dest => dest.ActionType, src => src.MapFrom(l => l.ActionType.Name))
                .ForMember(dest => dest.OldStatus, src => src.MapFrom(l => l.OldStatus.Status))
                .ForMember(dest => dest.NewStatus, src => src.MapFrom(l => l.NewStatus.Status))
                .ForMember(dest => dest.Owner, src => src.MapFrom(l => l.IsCreatedByAdmin ? l.CreatedByAdminNavigation.Name : l.CreatedByCustomerNavigation.Name));
            #endregion

            #endregion

            #region InvoiceAdmin  
            CreateMap<VersionSubscription, GetVersionsubscriptionInvoiceOutputDto>()
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(s => s.VersionPrice.VersionId))
                .ForMember(dest => dest.VersionSubscriptionId, opt => opt.MapFrom(s => s.Id))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(s => s.VersionRelease.Version.Image))
                .ForMember(dest => dest.PriceLevel, opt => opt.MapFrom(s => s.VersionPrice.PriceLevel))
                .ForMember(dest => dest.LicensesCount, opt => opt.MapFrom(s => s.CustomerSubscription.Licenses.Count))
                .ForMember(dest => dest.PriceLevels, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(s => s.VersionPrice.Version.Title))
                .ForMember(dest => dest.ApplicationName, opt => opt.MapFrom(s => s.VersionPrice.Version.Application.Title))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(s => s.VersionPrice.CountryCurrency.Currency.Code))
                .ForMember(dest => dest.Subscription, opt => opt.MapFrom(s => s.CustomerSubscription.SubscriptionType))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(s => s.CustomerSubscription.Invoices
                                                                                    .OrderByDescending(x => x.Id)
                                                                                    .FirstOrDefault(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(s => s.CustomerSubscription.Price))
                .ForMember(dest => dest.RenewDate, src => src.MapFrom<GetRenewalDateInvoiceResolver, int>(a => a.CustomerSubscriptionId));

            CreateMap<Customer, GetCustomerInvoiceOutputDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.Account, opt => opt.MapFrom(s => s.Contract.Serial))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(s => s.FullAddress))
                .ForMember(dest => dest.TaxReg, opt => opt.MapFrom(s => s.TaxRegistrationNumber));

            CreateMap<Customer, GetCustomerVersionOrAddOnOutputDto>()
                 .ForMember(dest => dest.Version, opt =>
                opt.Ignore())
                   .ForMember(dest => dest.AddOn, opt =>
                opt.Ignore());


            CreateMap<AddonSubscription, GetAddOnSubscriptionOutputDto>()
                 .ForMember(dest => dest.AddOnId, opt =>
                opt.MapFrom(s => s.AddonPrice.AddOnId))
                 .ForMember(dest => dest.VersionId, opt =>
                opt.MapFrom(s => s.VersionSubscription.VersionPrice.VersionId))
                  .ForMember(dest => dest.VersionReleaseId, opt =>
                opt.MapFrom(s => s.VersionSubscription.VersionReleaseId))
                .ForMember(dest => dest.AddOnsubscriptionId, opt =>
                opt.MapFrom(s => s.Id))
                 .ForMember(dest => dest.VersionSubscriptionId, opt =>
                opt.MapFrom(s => s.VersionSubscriptionId))
            .ForMember(dest => dest.Price, opt =>
            opt.MapFrom(x => x.CustomerSubscription.Price))
             .ForMember(dest => dest.Name, opt =>
            opt.MapFrom(x => x.AddonName))
            .ForMember(dest => dest.Logo, src =>
            src.MapFrom(a => a.AddonPrice.AddOn.Logo))
            .ForMember(dest => dest.PriceLevel, opt =>
                opt.MapFrom(s => s.AddonPrice.PriceLevel))
             .ForMember(dest => dest.ProductPrice, opt =>
                opt.MapFrom(s => s.CustomerSubscription.Invoices.OrderByDescending(x => x.Id).FirstOrDefault(x => x.InvoiceStatusId == (int)InvoiceStatusEnum.Paid)))
                 .ForMember(dest => dest.Currency, opt =>
                opt.MapFrom(s => s.AddonPrice.CountryCurrency.Currency.Code))
                  .ForMember(dest => dest.Subscription, opt =>
                opt.MapFrom(s => s.CustomerSubscription.SubscriptionType))
             .ForMember(dest => dest.RenewDate, src =>
                src.MapFrom<GetRenewalDateInvoiceResolver, int>(a => a.CustomerSubscriptionId));

            CreateMap<Invoice, GetInvoicePriceOutputDto>()
                 .ForMember(dest => dest.Discount, opt =>
            opt.MapFrom(x => x.InvoiceDetails.FirstOrDefault().Discount));

            CreateMap<AddOn, GetAddOnCanPurshasedOutputDto>()
              .ForMember(dest => dest.Logo, opts => opts.MapFrom(s => s.Logo))
              .ForMember(dest => dest.Name, opts => opts.MapFrom(s => s.Title))
              .ForMember(dest => dest.Pricelevel, opts => opts.Ignore())
              .ForMember(dest => dest.Price, src => src.Ignore());

            CreateMap<Version, GetVersionCanPurshasedOutputDto>()
             .ForMember(dest => dest.Logo, opts => opts.MapFrom(s => s.Image))
             .ForMember(dest => dest.Name, opts => opts.MapFrom(s => s.Title))
             .ForMember(dest => dest.ApplicationId, opts => opts.MapFrom(s => s.ApplicationId))
             .ForMember(dest => dest.ApplicationName, opts => opts.MapFrom(s => s.Application.Title))
             //.ForMember(dest => dest.Prices, opts => opts.MapFrom(s => s.VersionPrices))
             .ForMember(dest => dest.SubscriptionTypeId, opts => opts.MapFrom(s => s.Application.SubscriptionTypeId))
             .ForMember(dest => dest.VersionReleaseId, opts => opts.MapFrom(s => s.VersionReleases.FirstOrDefault(x => x.IsCurrent).Id))
             .ForMember(dest => dest.PriceLevels, opts => opts.Ignore())
             .ForMember(dest => dest.Price, src => src.Ignore());

            CreateMap<VersionPrice, RetrieveVersionPrice>()
             .ForMember(dest => dest.PriceLevel, opts => opts.MapFrom(s => s.PriceLevel))
             .ForMember(dest => dest.MonthlyPrice, opts => opts.MapFrom(s => s.MonthlyPrice))
             .ForMember(dest => dest.YearlyPrice, opts => opts.MapFrom(s => s.YearlyPrice))
             .ForMember(dest => dest.ForEverPrice, opts => opts.MapFrom(s => s.ForeverPrice))
             ;


            #endregion

            #region Auth.
            CreateMap<CustomerEmail, AlternativeEmailResultDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(e => e.CustomerId))
               .ForMember(dest => dest.EmailId, opt => opt.MapFrom(e => e.Id))
               .ForMember(dest => dest.BlockTillDate, opt =>
                    opt.MapFrom(e => e.BlockForSendingEmailUntil.HasValue ? DateTime.SpecifyKind(e.BlockForSendingEmailUntil.Value, DateTimeKind.Utc) : default))
               .ForMember(dest => dest.ExpiryDate, opt =>
                    opt.MapFrom(e => e.CustomerEmailVerifications.Any(v => v.ExpireDate > DateTime.UtcNow) ?
                                     DateTime.SpecifyKind(e.CustomerEmailVerifications.FirstOrDefault(v => v.ExpireDate > DateTime.UtcNow).ExpireDate, DateTimeKind.Utc) : default));

            CreateMap<CustomerMobile, AlternativeMobileResultDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(e => e.CustomerId))
               .ForMember(dest => dest.MobileId, opt => opt.MapFrom(e => e.Id))
               .ForMember(dest => dest.BlockTillDate, opt =>
                    opt.MapFrom(e => e.BlockForSendingSmsuntil.HasValue ? DateTime.SpecifyKind(e.BlockForSendingSmsuntil.Value, DateTimeKind.Utc) : default))
               .ForMember(dest => dest.ExpiryDate, opt =>
                    opt.MapFrom(e => e.CustomerSmsverifications.Any(v => v.ExpireDate > DateTime.UtcNow) ?
                                     DateTime.SpecifyKind(e.CustomerSmsverifications.FirstOrDefault(v => v.ExpireDate > DateTime.UtcNow).ExpireDate, DateTimeKind.Utc) : default));
            #endregion

            #region PaymentSetup
            CreateMap<PaymentMethod, GetPaymentSetupOutputDto>()
                .ForMember(dest => dest.PaymentSetupCredential, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<PaymentSetupCredential>(s.Credential)))
                .ForMember(dest => dest.CountryPayment, opt => opt.MapFrom(s => s.CountryPaymentMethods.FirstOrDefault()))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(s => s.PaymentType));


            CreateMap<PaymentMethod, GetPaymentMethodsOutputDto>()
               .ForMember(dest => dest.CountryPayment, opt => opt.MapFrom(s => s.CountryPaymentMethods.FirstOrDefault()))
               .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(s => s.PaymentType))
               .ForMember(dest => dest.Credential, opt => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.Credential) ? JsonConvert.DeserializeObject<PaymentSetupCredential>(s.Credential) : new PaymentSetupCredential()));

            CreateMap<PaymentType, PaymentTypeDto>();

            CreateMap<PaymentMethod, PaymentMethodDto>();

            CreateMap<CountryPaymentMethod, CountryPaymentDto>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom(s => s.Country.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.Country.Id));
            #endregion

            #region Invoices.
            //TODO :Recheck Mappings
            CreateMap<Customer, CustomerInfoDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Account, src => src.MapFrom(i => i.Contract != null ? i.Contract.Serial : string.Empty))
                .ForMember(dest => dest.CompanyName, src => src.MapFrom(i => i.CompanyName))
                .ForMember(dest => dest.TaxReg, src => src.MapFrom(i => i.TaxRegistrationNumber))
                .ForMember(dest => dest.Address, src => src.MapFrom(i => i.FullAddress))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Invoice, RetrieveInvoiceDto>()
                .ForMember(dest => dest.Discount, src => src.MapFrom(i => i.InvoiceDetails.FirstOrDefault().Discount))
                .ForMember(dest => dest.CancelReason, src => src.MapFrom(i => i.CancelReason))
                .ForMember(dest => dest.Currency, src => src.MapFrom(i => i.Currency != null ? i.Currency.Code : ""))
                .ForMember(dest => dest.PaymentMethod, src => src.MapFrom(i => i.PaymentMethod.Name))
                .ForMember(dest => dest.CreateDate, src => src.MapFrom(i => DateTime.SpecifyKind(i.CreateDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.PaymentDate, src => src.MapFrom(i => DateTime.SpecifyKind(i.PaymentDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.PaymentTypeId, src => src.MapFrom(i => i.PaymentMethod.PaymentTypeId))
                .ForMember(dest => dest.PaymentType, src => src.MapFrom(i => i.PaymentMethod.PaymentType.Name))
                .ForMember(dest => dest.InvoiceStatus, src => src.MapFrom(i => i.InvoiceStatus.Status))
                .ForMember(dest => dest.ProductTitle, src => src.MapFrom(i => i.InvoiceDetails.FirstOrDefault().VersionName))
                .ForMember(dest => dest.InvoiceTitle, src => src.MapFrom(i => i.InvoiceTitle))
                .ForMember(dest => dest.InvoiceTypeId, src => src.MapFrom(i => i.InvoiceTypeId))
                .ForMember(dest => dest.InvoiceType, src => src.MapFrom(i => i.InvoiceType.Type))
                .ForMember(dest => dest.Subscription, src => src.MapFrom(i => i.CustomerSubscription.SubscriptionType))
                .ForMember(dest => dest.Customer, src => src.MapFrom(i => i.CustomerSubscription.Customer))
                .ForMember(dest => dest.IsAddOn, src => src.MapFrom(i => i.CustomerSubscription.IsAddOn))
                .ForMember(dest => dest.PriceLevel, src => src.MapFrom(i => i.CustomerSubscription.IsAddOn
                               ? i.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonPrice.PriceLevel
                               : i.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionPrice.PriceLevel))
                 .ForMember(dest => dest.ExpirationDate, src => src.MapFrom(i => DateTime.SpecifyKind(i.EndDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.PaymentInfo, src => src.MapFrom(i => !string.IsNullOrWhiteSpace(i.PaymentInfo) ? JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(i.PaymentInfo) : new InvoicePaymentInfoJson { }))
                 .ForMember(dest => dest.CustomerSubscription, src => src.MapFrom(i => i.CustomerSubscription))
                 .ForMember(dest => dest.Discriminator, src => src.Ignore())
                 .ForMember(dest => dest.RenwalEvery, src => src.MapFrom(x => x.CustomerSubscription.RenewEvery))
                 .ForMember(dest => dest.VersionSubscription, src => src.Ignore())
                 .ForMember(dest => dest.AddOnSubscription, src => src.Ignore())
                 //.ForMember(dest => dest.CustomerData, src => src.MapFrom(i => i.CustomerSubscription.Customer))
                 //.ForMember(dest => dest.Customer.Name, src => src.MapFrom(i => i.CustomerSubscription.Customer.Name))
                 //.ForMember(dest => dest.Customer.TaxReg, src => src.MapFrom(i => i.CustomerSubscription.Customer.TaxRegistrationNumber))
                 //.ForMember(dest => dest.Customer.Account, src => src.MapFrom(i => i.CustomerSubscription.Customer.Contract.Id))
                 //.ForMember(dest => dest.Customer.Address, src => src.MapFrom(i => i.CustomerSubscription.Customer.FullAddress))
                 //TODO:Add data from dexef data
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CustomerSubscription, GetCustomerSubscriptionOutputDto>();
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Details, src => src.MapFrom(i => i.InvoiceDetails));
            CreateMap<InvoiceDetail, CreateInvoiceDetailInputDto>();


            //CreateMap<Invoice, GetApplicationPaymentDetailsOutputDto>()
            //   .ForMember(dest => dest.Discount, src => src.MapFrom(i => i.InvoiceDetails.FirstOrDefault().Discount))
            //   .ForMember(dest => dest.Currency, src => src.MapFrom(i => i.CustomerSubscription.CurrencyName))
            //   .ForMember(dest => dest.PaymentInfo, src => src.MapFrom(i => JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(i.PaymentInfo)));
            //CreateMap<APIGetCustomerDefaultTaxInputDto, GetCustomerDefaultTaxInputDto>()
            //      .ForMember(dest => dest.CustomerId, src => src.MapFrom(i => i.CustomerId));
            //       //   .ForMember(dest => dest.Tax, opts =>
            //       //opts.MapFrom<CountryDefaultTaxResolver, int>(src=>src.CustomerId)); 

            ;
            ;



            CreateMap<Invoice, InvoicePdfDto>()
              .ForMember(dest => dest.InvoiceItems, src => src.MapFrom(i => i.InvoiceDetails))
              .ForMember(dest => dest.CustomerInfo, src => src.MapFrom(i => i))
              .ForMember(dest => dest.InvoiceInfo, src => src.MapFrom(i => i))
              .ForMember(dest => dest.InvoiceSummary, src => src.MapFrom(i => i));

            CreateMap<Invoice, CustomerInfoPdfDto>()
               .ForMember(dest => dest.Name, src => src.MapFrom(i => i.CustomerSubscription.Customer.Name))
               .ForMember(dest => dest.Address, src => src.MapFrom(i => i.Address))
               .ForMember(dest => dest.TaxReg, src => src.MapFrom(i => i.TaxReg))
               .ForMember(dest => dest.ContractSerial, src => src.MapFrom(i => i.CustomerSubscription.Customer.Contract.Serial));

            CreateMap<Invoice, InvoiceInfoPdfDto>()
               .ForMember(dest => dest.Serial, src => src.MapFrom(i => i.Serial))
               .ForMember(dest => dest.Status, src => src.MapFrom(i => i.InvoiceStatus.Status))
               .ForMember(dest => dest.CreateDate, src => src.MapFrom(i => i.CreateDate.ToString("dd/MM/yyyy", new CultureInfo(Thread.CurrentThread.CurrentCulture.Name))))
               .ForMember(dest => dest.StartDate, src => src.MapFrom(i => i.StartDate.ToString("dd/MM/yyyy", new CultureInfo(Thread.CurrentThread.CurrentCulture.Name))))
               .ForMember(dest => dest.EndDate, src => src.MapFrom(i => i.EndDate.ToString("dd/MM/yyyy", new CultureInfo(Thread.CurrentThread.CurrentCulture.Name))));
            CreateMap<Invoice, InvoicePdfOutputDto>()
            .ForMember(dest => dest.InvoiceItems, src => src.MapFrom(i => i.InvoiceDetails))
              .ForMember(dest => dest.CustomerInfo, src => src.MapFrom(i => i))
              .ForMember(dest => dest.InvoiceInfo, src => src.MapFrom(i => i))
              .ForMember(dest => dest.InvoiceSummary, src => src.MapFrom(i => i))
              .ForMember(dest => dest.StatusId, src => src.MapFrom(i => i.InvoiceStatusId))
              .ForMember(dest => dest.LogoPath, src => src.Ignore());




            CreateMap<Invoice, InvoiceSummaryPdfDto>()
               .ForMember(dest => dest.VatPercentage, src => src.MapFrom(i => i.VatPercentage))
               .ForMember(dest => dest.VatValue, src => src.MapFrom(i => i.TotalVatAmount))
               .ForMember(dest => dest.Subtotal, src => src.MapFrom(i => i.SubTotal))
               .ForMember(dest => dest.Total, src => src.MapFrom(i => i.Total))
               .ForMember(dest => dest.CurrencyName, src => src.MapFrom(i => i.CustomerSubscription.CurrencyName));

            CreateMap<InvoiceDetail, InvoiceItemPdfDto>()
                .ForMember(dest => dest.ProductName, src => src.MapFrom(d => d.VersionName))
                .ForMember(dest => dest.PriceLevel, src => src.MapFrom(d => d.Invoice.CustomerSubscription.NumberOfLicenses))
                /*Invoice.CustomerSubscription.IsAddOn ?*/
                //d.Invoice.CustomerSubscription.AddonSubscriptions.FirstOrDefault().AddonPrice.PriceLevel.Name :
                //d.Invoice.CustomerSubscription.VersionSubscriptions.FirstOrDefault().VersionPrice.PriceLevel.Name))
                .ForMember(dest => dest.SubscriptionType, src => src.MapFrom(d => d.Invoice.CustomerSubscription.SubscriptionType.Name))
                .ForMember(dest => dest.CurrencyName, src => src.MapFrom(d => d.Invoice.CustomerSubscription.CurrencyName))
                .ForMember(dest => dest.NetPrice, src => src.MapFrom(d => d.NetPrice))
                .ForMember(dest => dest.Discount, src => src.MapFrom(d => d.Discount));

            CreateMap<Invoice, TicketInvoiceLookupDto>()
                .ForMember(dest => dest.Title, src => src.MapFrom(i => i.InvoiceTitle))
                .ForMember(dest => dest.Currency, src => src.MapFrom(i => i.CustomerSubscription.CurrencyName))
                .ForMember(dest => dest.Price, src => src.MapFrom(i => i.Total));

            CreateMap<Invoice, NearestVersionRenewDto>()
                .ForMember(dest => dest.NearestRenewDate, src => src.MapFrom(i => i.InvoiceTypeId == (int)InvoiceTypes.Support ? i.StartDate : i.EndDate))
                .ForMember(dest => dest.Amount, src => src.MapFrom(i => i.Total))
                .ForMember(dest => dest.Currency, src => src.MapFrom(i => i.Currency.Code));

            #endregion

            #region Role
            CreateMap<Role, GetRoleOutputDto>()
               .ForMember(dest => dest.Status, src => src.MapFrom(i => i.IsActive))
               .ForMember(dest => dest.RolePageActions, src => src.Ignore())
               .ForMember(dest => dest.PagesActions, src => src.Ignore())
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RolePageAction, GetRolePageActionOutputDto>()
                 //.ForMember(dest => dest.Page, src => src.MapFrom(i => i.PageAction.Page))
                 //.ForMember(dest => dest.Actions, src => src.Ignore())
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Page, PageDto>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Action, ActionDto>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Page, GetPageActionOutputDto>()
                 .ForMember(dest => dest.PageId, src => src.MapFrom(i => i.Id))
                 .ForMember(dest => dest.ActionIds, src => src.Ignore())
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region CustomerCard
            CreateMap<CustomerCard, CardTokenDto>()
                  .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(i => i.CardNumber.Substring(i.CardNumber.Length - 4)))
                  .ForMember(dest => dest.CallBackUrl, opt => opt.Ignore());
            #endregion
            #region Log
            CreateMap<AuditActionType, AuditActionTypeDto>();
            CreateMap<AuditLogDetail, CustomerActivitesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(i => i.AuditLog.PrimaryKey))
                 .ForMember(dest => dest.Field, opt => opt.MapFrom(i => i.FieldName))
                 .ForMember(dest => dest.Entity, opt => opt.MapFrom(i => i.AuditLog.TableName))
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(i => i.AuditLog.PrimaryKey))
                .ForMember(dest => dest.ActionType, opt => opt.MapFrom(i => i.AuditLog.AuditActionType))
                 .ForMember(dest => dest.Owner, opt => opt.Ignore())
                 .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(i => i.AuditLog.CreateDate));

            CreateMap<AuditLogDetail, TaxActivitesDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(i => i.AuditLog.PrimaryKey))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(i => i.FieldName))
                .ForMember(dest => dest.Entity, opt => opt.MapFrom(i => i.AuditLog.TableName))
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(i => i.AuditLog.PrimaryKey))
               .ForMember(dest => dest.ActionType, opt => opt.MapFrom(i => i.AuditLog.AuditActionType))
                .ForMember(dest => dest.AuditLogId, opt => opt.MapFrom(i => i.AuditLogId))
                 .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(i => i.AuditLog.CreateDate));
            #endregion

            #region WorkSpaces
            CreateMap<CurrencyTable, DexefCountryDto>();
            CreateMap<CurrencyTable, DexefCurrencyDto>();

            CreateMap<Workspace, WorkspaceDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom<BlobURLResolver, FileStorage>(s => s.Image));

            CreateMap<Workspace, WorkspaceDetailsDto>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(w => w.DexefCountryId))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(w => w.DexefCurrencyId));


            CreateMap<SimpleDatabas, SimpleDatabaseDto>();


            #endregion

            #region Devices Type
            CreateMap<DevicesType, GetDevicesTypeDto>();
            CreateMap<DevicesType, GetDevicesTypeOutputDto>();
            #endregion


            #region Ticket

            CreateMap<Ticket, GetTicketDto>()
                .ForMember(des => des.StatusId, opt => opt.MapFrom(e => e.Status.Id))
                .ForMember(des => des.Status, opt => opt.MapFrom(e => e.Status.Status))
                .ForMember(des => des.TypeId, opt => opt.MapFrom(e => e.Type.Id))
                .ForMember(des => des.Type, opt => opt.MapFrom(e => e.Type.Status));

            #endregion


            #region chatMessage

            CreateMap<ChatMessage, MessageResultDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(e => e.Id))
                .ForMember(dest => dest.Message, src => src.MapFrom(e => e.Message))
                .ForMember(dest => dest.SendTime, src => src.MapFrom(e => e.SendTime))
                .ForMember(dest => dest.ReadTime, src => src.MapFrom(e => e.ReadTime))
                .ForMember(dest => dest.Name, src => src.MapFrom<ChatMessageNameResolver, bool>(e => e.IsCustomer))
                //  .ForMember(dest => dest.Name, src => src.MapFrom(e=>e.IsCustomer?e.Ticket.Customer.Name:"Technical Support"))
                .ForMember(dest => dest.TicketId, src => src.MapFrom(e => e.TicketId))
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom<ChatMessageMediaResolver, InputFileDto>(
                    src => new InputFileDto { EntityId = src.Id, TableName = TableNameEnum.ChatMessage }))
                ;

            CreateMap<FileStorage, MediaDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom<CustomBlobURLResolver, string>(src => src.FullPath))
                .ForMember(dest => dest.Extention, opt => opt.MapFrom(e => e.Extension))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(e => e.FileType.Name));
            #endregion


            #region DownloadCenter
            CreateMap<Application, DownloadCenterApplicationResultDto>()
                .ForMember(dest => dest.subscribtionTypeId
                  , opts => opts.MapFrom(s => s.SubscriptionTypeId))
                  .ForMember(dest => dest.Label
                  , opts => opts.MapFrom(s => s.ApplicationLabels.FirstOrDefault()))
                  .ForMember(dest => dest.Description
                  , opts => opts.MapFrom(s => s.ShortDescription))
                   .ForMember(dest => dest.Logo
                  , opts => opts.MapFrom(s => s.Image))
                   .ForMember(dest => dest.CurrencyCode
                  , opts => opts.MapFrom<CurrencyCodeResolver>())
                   .ForMember(dest => dest.Price
                  , opts => opts.MapFrom<DownloadCenterPriceResolver, Application>(a => a))
                  //         .ForMember(dest => dest.CurrencyCode,
                  //opts => opts.MapFrom(s =>
                  //    s.Versions
                  //    .SelectMany(v => v.VersionPrices.Select(vp => vp.CountryCurrency.Currency.Code))
                  //    .FirstOrDefault()))

                  .ForMember(dest => dest.Rate, src => src.MapFrom<ApplicationReviewResolver, int>(a => a.Id))
                ;
            CreateMap<AddOn, DownloadCenterAddonResultDto>()
                  .ForMember(dest => dest.ApplicationId
                          , opts => opts.MapFrom(s => s.VersionAddons.Where(x => x.Version != null && x.Version.ApplicationId > 0).Select(va => va.Version.ApplicationId).Distinct()))
                  .ForMember(dest => dest.Label
                   , opts => opts.MapFrom(s => s.AddOnLabels.FirstOrDefault()))
                  .ForMember(dest => dest.Description
                          , opts => opts.MapFrom(s => s.ShortDescription))
                  .ForMember(dest => dest.Logo
                          , opts => opts.MapFrom(s => s.Logo))
                  .ForMember(dest => dest.CurrencyCode
                  , opts => opts.MapFrom<CurrencyCodeResolver>())
                  .ForMember(dest => dest.Price
                  , opts => opts.MapFrom<DownloadCenterAddOnPriceResolver, AddOn>(a => a))
                  ;

            //.ForMember(dest => dest.CurrencyCode,
            //           opts => opts.MapFrom(s =>
            //           s.AddOnPrices.Select(a => a.CountryCurrency.Currency.Code).FirstOrDefault()));


            #endregion
        }


    }
}
