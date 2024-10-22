using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Core.Entities;
using Version = Ecommerce.Core.Entities.Version;
namespace Ecommerce.Core.Infrastructure
{
    public static class DataBaseSeed
    {
        //seed data work work in case of Database First
        public static void SeedLockUpData(this ApplicationDbContext db)
        {
            bool flagUpdateDb = false;
            //check if no data in tables
            if (!db.CompanySizes.Any())
            {
                flagUpdateDb = true;
                List<CompanySize> CompanySizeslist = new List<CompanySize>();
                CompanySizeslist.Add(new CompanySize { Id = 1, Size = "1-5", IsActive = true, CreateDate = DateTime.UtcNow, Crmid = 1 });
                CompanySizeslist.Add(new CompanySize { Id = 2, Size = "6-25", IsActive = true, CreateDate = DateTime.UtcNow, Crmid = 2 });
                CompanySizeslist.Add(new CompanySize { Id = 3, Size = "26-50", IsActive = true, CreateDate = DateTime.UtcNow, Crmid = 3 });
                CompanySizeslist.Add(new CompanySize { Id = 4, Size = "51-100", IsActive = true, CreateDate = DateTime.UtcNow, Crmid = 4 });
                CompanySizeslist.Add(new CompanySize { Id = 5, Size = "101-500", IsActive = true, CreateDate = DateTime.UtcNow, Crmid = 5 });
                db.CompanySizes.AddRange(CompanySizeslist);
            }
            if (!db.ContactUsHelpOptions.Any())
            {
                flagUpdateDb = true;
                List<ContactUsHelpOption> ContactUsHelpOptionList = new List<ContactUsHelpOption>();
                ContactUsHelpOptionList.Add(new ContactUsHelpOption { Id = 1, HelpName = "{\"default\":\"I'd like a quote\",\"\ar\":\"أود الحصول على عرض أسعار\"}", IsActive = true, CreateDate = DateTime.UtcNow });
                ContactUsHelpOptionList.Add(new ContactUsHelpOption { Id = 2, HelpName = "{\"default\":\"I'd like a demo\",\"\ar\":\"أود الحصول على عرض توضيحى\"}", IsActive = true, CreateDate = DateTime.UtcNow, });
                ContactUsHelpOptionList.Add(new ContactUsHelpOption { Id = 3, HelpName = "{\"default\":\"I'd like a more information about functionality\",\"\ar\":\"اود الحصول على مزيد من المعلومات حول كيفية عمل النظام\"}", IsActive = true, CreateDate = DateTime.UtcNow, });
                ContactUsHelpOptionList.Add(new ContactUsHelpOption { Id = 4, HelpName = "{\"default\":\"I'm an existing user and need support\",\"\ar\":\"أنا مستخدم حالي وأحتاج إلى دعم فنى\"}", IsActive = true, CreateDate = DateTime.UtcNow, });
                ContactUsHelpOptionList.Add(new ContactUsHelpOption { Id = 5, HelpName = "{\"default\":\"Other\",\"\ar\":\"اخري\"}", IsActive = true, CreateDate = DateTime.UtcNow, });
                db.ContactUsHelpOptions.AddRange(ContactUsHelpOptionList);
            }
            if (!db.Currencies.Any())
            {
                flagUpdateDb = true;
                List<Currency> CurrencyList = new List<Currency>();
                CurrencyList.Add(new Currency { Id = 1, Name = "{\"default\":\"Egyptian Pound\",\"\ar\":\"جنية مصرى\"}", Code = "EGP", Symbole = "£", Crmid = "53721e13-2209-e811-80dc-001dd8d72a68", IsActive = true });
                CurrencyList.Add(new Currency { Id = 2, Name = "{\"default\":\"Saudi Arabia Riyal\",\"\ar\":\"ريال سعودى\"}", Code = "SAR", Symbole = "﷼", Crmid = "f4155f2a-460a-e811-80de-001dd8d72a68L", IsActive = true });
                CurrencyList.Add(new Currency { Id = 3, Name = "{\"default\":\"Euro\",\"\ar\":\"يورو\"}", Code = "EUR", Symbole = "€", Crmid = "7d60cf80-460a-e811-80de-001dd8d72a68", IsActive = true });
                CurrencyList.Add(new Currency { Id = 4, Name = "{\"default\":\"United States Dollar\",\"\ar\":\"دولار أمريكي\"}", Code = "USD", Symbole = "$", IsActive = true });
                CurrencyList.Add(new Currency { Id = 5, Name = "{\"default\":\"Iraqi Dinar\",\"\ar\":\"دينار عراقى\"}", Code = "IQD", Symbole = "ع.د", IsActive = true });
                CurrencyList.Add(new Currency { Id = 6, Name = "{\"default\":\"Euro\",\"\ar\":\"يورو\"}", Code = "EUR", Symbole = "€", Crmid = "7d60cf80-460a-e811-80de-001dd8d72a68", IsActive = true });
                CurrencyList.Add(new Currency { Id = 7, Name = "{\"default\":\"United States Dollar\",\"\ar\":\"دولار أمريكي\"}", Code = "USD", Symbole = "$", IsActive = true });
                db.Currencies.AddRange(CurrencyList);
            }
            if (!db.CustomerReviewStatus.Any())
            {
                flagUpdateDb = true;
                List<CustomerReviewStatu> CustomerReviewStatusList = new List<CustomerReviewStatu>();
                CustomerReviewStatusList.Add(new CustomerReviewStatu { Id = 1, Status = "Pending" });
                CustomerReviewStatusList.Add(new CustomerReviewStatu { Id = 2, Status = "Confirmed" });
                CustomerReviewStatusList.Add(new CustomerReviewStatu { Id = 3, Status = "Rejected" });
                db.CustomerReviewStatus.AddRange(CustomerReviewStatusList);
            }
            if (!db.CustomerStatus.Any())
            {
                flagUpdateDb = true;
                List<CustomerStatu> customerStatus = new List<CustomerStatu>();
                customerStatus.Add(new CustomerStatu { Id = 1, Status = "Unverified" });
                customerStatus.Add(new CustomerStatu { Id = 2, Status = "Verified" });
                customerStatus.Add(new CustomerStatu { Id = 3, Status = "Pending" });
                customerStatus.Add(new CustomerStatu { Id = 4, Status = "Registered" });
                customerStatus.Add(new CustomerStatu { Id = 5, Status = "Suspended" });
                db.CustomerStatus.AddRange(customerStatus);
            }
            if (!db.SubscriptionTypes.Any())
            {
                flagUpdateDb = true;
                List<SubscriptionType> subtypes = new List<SubscriptionType>();
                subtypes.Add(new SubscriptionType { Id = 1, Name = "{\"default\":\"ForEver\",\"\ar\":\"للابد\"}", });
                subtypes.Add(new SubscriptionType { Id = 2, Name = "{\"default\":\"Others\",\"\ar\":\"أخري\"}", });
                db.SubscriptionTypes.AddRange(subtypes);
            }
            if (!db.PriceLevels.Any())
            {
                flagUpdateDb = true;
                List<PriceLevel> priceLevels = new List<PriceLevel>();
                priceLevels.Add(new PriceLevel { Id = 1, Name = "{\"default\":\"high\",\"\ar\":\"السوبر\"}", NumberOfLicenses=3,IsActive = true, IsDeleted = false });
                priceLevels.Add(new PriceLevel { Id = 2, Name = "{\"default\":\"medium\",\"\ar\":\"المتوسط\"}", NumberOfLicenses = 1, IsActive=true,IsDeleted=false });
                db.PriceLevels.AddRange(priceLevels);
            }
            if (!db.Countries.Any())
            {
                flagUpdateDb = true;
                List<Country> CountriesList = new List<Country>();
                CountriesList.Add(new Country { Id = 1, Name = "{\"default\":\"Egypt\",\"\ar\":\"مصر\"}", IsActive = true, IsDeleted = false });
                CountriesList.Add(new Country { Id = 2, Name = "{\"default\":\"Saudi Arabia\",\"\ar\":\"السعودية\"}", IsActive = true, IsDeleted = false });
                db.Countries.AddRange(CountriesList);
            }
            if (!db.CountryCurrencies.Any())
            {
                flagUpdateDb = true;
                List<CountryCurrency> CountriesList = new List<CountryCurrency>();
                CountriesList.Add(new CountryCurrency { Id = 1, CountryId = 1, IsActive = true, IsDeleted = false });
                CountriesList.Add(new CountryCurrency { Id = 2, CountryId = 2, IsActive = true, IsDeleted = false });
                db.CountryCurrencies.AddRange(CountriesList);
            }

            if (!db.LicenseStatus.Any())
            {
                flagUpdateDb = true;
                List<LicenseStatu> licencesStatus = new List<LicenseStatu>();
                licencesStatus.Add(new LicenseStatu { Id = 1, Status = "{\"default\":\"InProgress\",\"\ar\":\"قيد الانتظار\"}", IsActive = true, IsDeleted = false });
                licencesStatus.Add(new LicenseStatu { Id = 2, Status = "{\"default\":\"Generated\",\"\ar\":\"مفعلة\"}", IsActive = true, IsDeleted = false });
                licencesStatus.Add(new LicenseStatu { Id = 3, Status = "{\"default\":\"Expired\",\"\ar\":\"منتهية الصلاحية\"}", IsActive = true, IsDeleted = false });
                licencesStatus.Add(new LicenseStatu { Id = 4, Status = "{\"default\":\"Changed\",\"\ar\":\"مستبدلة\"}", IsActive = true, IsDeleted = false });
                db.LicenseStatus.AddRange(licencesStatus);
            }

            if (!db.InvoiceStatus.Any())
            {
                flagUpdateDb = true;
                List<InvoiceStatu> InvoiceStatusList = new List<InvoiceStatu>();
                InvoiceStatusList.Add(new InvoiceStatu { Id = 1, Status = "{\"default\":\"UnPaid\",\"\ar\":\"غير مدفوعة\"}", IsActive = true, IsDeleted = false });
                InvoiceStatusList.Add(new InvoiceStatu { Id = 2, Status = "{\"default\":\"Paid\",\"\ar\":\"مدفوعة\"}", IsActive = true, IsDeleted = false });
                InvoiceStatusList.Add(new InvoiceStatu { Id = 3, Status = "{\"default\":\"Draft\",\"\ar\":\"مسودة\"}", IsActive = true, IsDeleted = false });
                InvoiceStatusList.Add(new InvoiceStatu { Id = 4, Status = "{\"default\":\"Refunded\",\"\ar\":\"معادة\"}", IsActive = true, IsDeleted = false });
                InvoiceStatusList.Add(new InvoiceStatu { Id = 5, Status = "{\"default\":\"Canceled\",\"\ar\":\"ملغية\"}", IsActive = true, IsDeleted = false });
                InvoiceStatusList.Add(new InvoiceStatu { Id = 6, Status = "{\"default\":\"WaitingPaymentConfirmation\",\"\ar\":\"في انتظار تأكيد الدفع\"}", IsActive = true, IsDeleted = false });
                db.InvoiceStatus.AddRange(InvoiceStatusList);
            }
            if (!db.InvoiceTypes.Any())
            {
                flagUpdateDb = true;
                List<InvoiceType> InvoiceTypeslist = new List<InvoiceType>();
                InvoiceTypeslist.Add(new InvoiceType { Id = 1, Type = "Support",IsActive = true, IsDeleted = false });
                InvoiceTypeslist.Add(new InvoiceType { Id = 2, Type = "Renewal", IsActive = true, IsDeleted = false });
                InvoiceTypeslist.Add(new InvoiceType { Id = 3, Type = "Forever Subscription", IsActive = true, IsDeleted = false });
                db.InvoiceTypes.AddRange(InvoiceTypeslist);
            }

            if (!db.PaymentTypes.Any())
            {
                flagUpdateDb = true;
                List<PaymentType> PaymentTypeslist = new List<PaymentType>();
                PaymentTypeslist.Add(new PaymentType { Id = 1, Name = "Paypal", IsActive = true, IsDeleted = false });
                PaymentTypeslist.Add(new PaymentType { Id = 2, Name = "Fawry", IsActive = true, IsDeleted = false });
                PaymentTypeslist.Add(new PaymentType { Id = 3, Name = "Visa", IsActive = true, IsDeleted = false });
                PaymentTypeslist.Add(new PaymentType { Id = 4, Name = "Wait Cash Refund", IsActive = true, IsDeleted = false });
                PaymentTypeslist.Add(new PaymentType { Id = 5, Name = "Cash", IsActive = true, IsDeleted = false });
                db.PaymentTypes.AddRange(PaymentTypeslist);
            }
            if (!db.PaymentMethods.Any())
            {
                flagUpdateDb = true;
                List<PaymentMethod> PaymentMethodlist = new List<PaymentMethod>();
                PaymentMethodlist.Add(new PaymentMethod { Id = 1,PaymentTypeId=1, Name = "Paypal", IsActive = true, IsDeleted = false });               
                db.PaymentMethods.AddRange(PaymentMethodlist);
            }

            if (!db.Applications.Any())
            {
                flagUpdateDb = true;
                var app =
                   new Application()
                   {
                       IsActive = true,
                       Name = "{\"default\":\"AppTest\",\"\ar\":\"تست ابلكيشن\"}",
                       Title = "{\"default\":\"AppTest\",\"\ar\":\"تست ابلكيشن\"}",
                       ShortDescription = "{\"default\":\"AppTest\",\"\ar\":\"تست ابلكيشن\"}",
                       LongDescription = "{\"default\":\"AppTest\",\"\ar\":\"تست ابلكيشن\"}",
                       SubscriptionTypeId = 1,
                       IsDeleted = false,
                       CreateDate = DateTime.UtcNow,
                       Versions=new List<Version>()
                       {
                           new Version()
                           {
                                IsActive = true,
                                Name = "{\"default\":\"VerTest\",\"\ar\":\"تست ففيرجن\"}",
                                Title = "{\"default\":\"VerTest\",\"\ar\":\"تست فيرجن\"}",
                                ShortDescription = "{\"default\":\"VerTest\",\"\ar\":\"تست فيرجن\"}",
                                LongDescription = "{\"default\":\"VerTest\",\"\ar\":\"تست فيرجن\"}",
                                IsDeleted = false,
                                CreateDate = DateTime.UtcNow,
                                VersionModules=new List<VersionModule>()
                                {
                                    new VersionModule()
                                    {
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreateDate = DateTime.UtcNow,
                                        Module=new Module()
                                        {
                                             IsActive = true,
                                             Name = "{\"default\":\"moduleTest\",\"\ar\":\"تست موديول\"}",
                                             Title = "{\"default\":\"moduleTest\",\"\ar\":\"تست موديول\"}",
                                             ShortDescription = "{\"default\":\"moduleTest\",\"\ar\":\"تست موديول\"}",
                                             LongDescription = "{\"default\":\"moduleTest\",\"\ar\":\"تست موديول\"}",
                                             IsDeleted = false,
                                             CreateDate = DateTime.UtcNow,
                                        }
                                    }
                                },
                                VersionPrices=new List<VersionPrice>()
                                {
                                    new VersionPrice()
                                    {
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreateDate = DateTime.UtcNow,
                                        PriceLevelId=1,
                                        CountryCurrencyId=1,
                                        YearlyPrice =15000,
                                        YearlyPrecentageDiscount =5,
                                        YearlyNetPrice =10000,
                                        MonthlyPrice =1000,
                                        MonthlyPrecentageDiscount =0,
                                        MonthlyNetPrice =1000,
                                        ForeverPrice =50000,
                                        ForeverPrecentageDiscount =10,
                                        ForeverNetPrice =40000,
                                    }
                                },
                                VersionReleases=new List<VersionRelease>()
                                {
                                    new VersionRelease()
                                    {
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreateDate = DateTime.UtcNow,
                                        ReleaseNumber="100",
                                        DownloadUrl="https://example.com/",
                                        IsCurrent=true,
                                    }
                                },
                                


                           }
                       },
                       
                      
                   };
                db.Applications.Add(app);
            }


            if (!db.Employees.Any())
            {
                flagUpdateDb = true;
                var empSuperAdmin =
                   new Employee()
                   {
                       IsActive = true,
                       UserName = "admin",
                       Password = "sha1:64000:18:f3kspDJV+GaraO3dp5P/iaDehI2ea6jE:g6Z3fpr45eOyHJ9i/Bm5B+xK",
                       IsAdmin = true,
                       IsDeleted = false,
                       CreateDate = DateTime.UtcNow,
                   }
                     ;
                db.Employees.Add(empSuperAdmin);
            }

            if (!db.Customers.Any())
            {
                flagUpdateDb = true;
                var customer =
                  new Customer()
                  {

                      CompanyName = "testcompany",
                      Name = "testcust",
                      IsActive = true,
                      Password = "sha1:64000:18:f3kspDJV+GaraO3dp5P/iaDehI2ea6jE:g6Z3fpr45eOyHJ9i/Bm5B+xK",
                      IsDeleted = false,
                      CreateDate = DateTime.UtcNow,
                      CustomerEmails = new List<CustomerEmail>()
                      {
                          new CustomerEmail()
                          {
                              CreateDate = DateTime.UtcNow,
                              Email = "ahmed.derblaa@dexef.net",
                              IsVerified = true,
                              IsPrimary = true,
                          }
                      },
                      CustomerSubscriptions = new List<CustomerSubscription>()
                      {
                          new CustomerSubscription()
                          {
                              CreateDate=DateTime.UtcNow,
                              IsActive=true,
                              IsDeleted=false,
                              SubscriptionTypeId=2,
                              CurrencyName = "{\"default\":\"EGP\",\"\ar\":\"ج.م\"}",
                              IsAddOn=false,
                              Price=500,
                              PriceAfterDiscount=500,
                              RenewEvery=365,
                              NumberOfLicenses=3,
                              AutoBill=true,
                              VersionSubscriptions= new List<VersionSubscription>()
                              {
                               new VersionSubscription()
                               {
                                   CreateDate=DateTime.UtcNow,
                                   IsActive=true,
                                   IsDeleted=false,
                                   VersionPriceId=1,
                                   VersionReleaseId =1,
                                   VersionName=  "{\"default\":\"VerTest\",\"\ar\":\"تست ففيرجن\"}",
                                   
                               }
                              },
                              Licenses= new List<License>()
                              {
                               new License()
                               {
                                   CreateDate=DateTime.UtcNow,
                                   IsActive=true,
                                   IsDeleted=false,
                                   Serial="testnumber12345678",
                                   LicenseStatusId =3,
                                   DeviceName=  "{\"default\":\"DevTest\",\"\ar\":\"تست جهاز\"}",
                               }
                              },
                              Invoices=new List<Invoice>()
                              {
                               new Invoice()
                               {
                                   CreateDate=DateTime.UtcNow,
                                   IsActive=true,
                                   IsDeleted=false,
                                   Serial="testnumSerialNumber123456",
                                   InvoiceTitle ="TitleTest",
                                   InvoiceStatusId=  2,
                                   InvoiceTypeId=1,
                                   PaymentMethodId=1
                               }
                              },
                          }
                      },
                     


                  }
                     ;
                db.Customers.Add(customer);
            }

            if (flagUpdateDb)
                db.SaveChanges();

           // db.RE
        }
    }
}
