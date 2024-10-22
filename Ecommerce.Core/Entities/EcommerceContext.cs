using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Ecommerce.Core.Entities
{
    public partial class EcommerceContext : DbContext
    {
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<AddOn> AddOns { get; set; }
        public virtual DbSet<AddOnLabel> AddOnLabels { get; set; }
        public virtual DbSet<AddOnPrice> AddOnPrices { get; set; }
        public virtual DbSet<AddOnSlider> AddOnSliders { get; set; }
        public virtual DbSet<AddOnTag> AddOnTags { get; set; }
        public virtual DbSet<AddonSubscription> AddonSubscriptions { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<ApplicationLabel> ApplicationLabels { get; set; }
        public virtual DbSet<ApplicationSlider> ApplicationSliders { get; set; }
        public virtual DbSet<ApplicationTag> ApplicationTags { get; set; }
        public virtual DbSet<AuditActionType> AuditActionTypes { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<AuditLogDetail> AuditLogDetails { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<CompanySize> CompanySizes { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
        public virtual DbSet<ContactUsHelpOption> ContactUsHelpOptions { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryCurrency> CountryCurrencies { get; set; }
        public virtual DbSet<CountryPaymentMethod> CountryPaymentMethods { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyTable> CurrencyTables { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCard> CustomerCards { get; set; }
        public virtual DbSet<CustomerEmail> CustomerEmails { get; set; }
        public virtual DbSet<CustomerEmailVerification> CustomerEmailVerifications { get; set; }
        public virtual DbSet<CustomerMobile> CustomerMobiles { get; set; }
        public virtual DbSet<CustomerReview> CustomerReviews { get; set; }
        public virtual DbSet<CustomerReviewStatu> CustomerReviewStatus { get; set; }
        public virtual DbSet<CustomerSmsverification> CustomerSmsverifications { get; set; }
        public virtual DbSet<CustomerStatu> CustomerStatus { get; set; }
        public virtual DbSet<CustomerSubscription> CustomerSubscriptions { get; set; }
        public virtual DbSet<DevicesType> DevicesTypes { get; set; }
        public virtual DbSet<DexefBranch> DexefBranches { get; set; }
        public virtual DbSet<DownloadAddOnLog> DownloadAddOnLogs { get; set; }
        public virtual DbSet<DownloadVersionLog> DownloadVersionLogs { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeCountry> EmployeeCountries { get; set; }
        public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<FileStorage> FileStorages { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<GeneratedNorFile> GeneratedNorFiles { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Industry> Industries { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<InvoiceStatu> InvoiceStatus { get; set; }
        public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<License> Licenses { get; set; }
        public virtual DbSet<LicenseActionType> LicenseActionTypes { get; set; }
        public virtual DbSet<LicenseFile> LicenseFiles { get; set; }
        public virtual DbSet<LicenseLog> LicenseLogs { get; set; }
        public virtual DbSet<LicenseStatu> LicenseStatus { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModuleSlider> ModuleSliders { get; set; }
        public virtual DbSet<ModuleTag> ModuleTags { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationAction> NotificationActions { get; set; }
        public virtual DbSet<NotificationActionPage> NotificationActionPages { get; set; }
        public virtual DbSet<NotificationActionSubType> NotificationActionSubTypes { get; set; }
        public virtual DbSet<NotificationActionType> NotificationActionTypes { get; set; }
        public virtual DbSet<OauthType> OauthTypes { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageAction> PageActions { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<PriceLevel> PriceLevels { get; set; }
        public virtual DbSet<ReasonChangeDevice> ReasonChangeDevices { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<RefundRequest> RefundRequests { get; set; }
        public virtual DbSet<RefundRequestStatu> RefundRequestStatus { get; set; }
        public virtual DbSet<RegistrationSource> RegistrationSources { get; set; }
        public virtual DbSet<RequestActivationKey> RequestActivationKeys { get; set; }
        public virtual DbSet<RequestActivationKeyStatu> RequestActivationKeyStatus { get; set; }
        public virtual DbSet<RequestChangeDevice> RequestChangeDevices { get; set; }
        public virtual DbSet<RequestChangeDeviceStatu> RequestChangeDeviceStatus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePageAction> RolePageActions { get; set; }
        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<SimpleDatabas> SimpleDatabases { get; set; }
        public virtual DbSet<Smstype> Smstypes { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public virtual DbSet<TableName> TableNames { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketStatu> TicketStatus { get; set; }
        public virtual DbSet<TicketType> TicketTypes { get; set; }
        public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<VersionAddon> VersionAddons { get; set; }
        public virtual DbSet<VersionFeature> VersionFeatures { get; set; }
        public virtual DbSet<VersionModule> VersionModules { get; set; }
        public virtual DbSet<VersionPrice> VersionPrices { get; set; }
        public virtual DbSet<VersionRelease> VersionReleases { get; set; }
        public virtual DbSet<VersionSubscription> VersionSubscriptions { get; set; }
        public virtual DbSet<ViewApplicationVersionFeature> ViewApplicationVersionFeatures { get; set; }
        public virtual DbSet<ViewDashboardTotal> ViewDashboardTotals { get; set; }
        public virtual DbSet<ViewLicenseRequest> ViewLicenseRequests { get; set; }
        public virtual DbSet<ViewLicenseRequestsForValy> ViewLicenseRequestsForValies { get; set; }
        public virtual DbSet<ViewMissingAddOnPrice> ViewMissingAddOnPrices { get; set; }
        public virtual DbSet<ViewMissingVersionPrice> ViewMissingVersionPrices { get; set; }
        public virtual DbSet<WishListAddOn> WishListAddOns { get; set; }
        public virtual DbSet<WishListApplication> WishListApplications { get; set; }
        public virtual DbSet<WorkSpaceStatu> WorkSpaceStatus { get; set; }
        public virtual DbSet<Workspace> Workspaces { get; set; }

        public EcommerceContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Data source=tcp:Ecommerce-db-server.database.windows.net,1433;initial catalog=EcommerceTest;Integrated security=False;User ID=dexefAdmin;Password=Admin#123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Action>(entity =>
            {
                entity.ToTable("Action", "Admin");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<AddOn>(entity =>
            {
                entity.ToTable("AddOn", "Admin");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.HasIndex(e => e.Title)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DownloadUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.LongDescription)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.MainPageUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AddOnCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOn_EmployeeCreated");

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.AddOns)
                    .HasForeignKey(d => d.LogoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOn_File");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.AddOnModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AddOn_EmployeeModified");
            });

            modelBuilder.Entity<AddOnLabel>(entity =>
            {
                entity.ToTable("AddOnLabel", "Admin");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.AddOn)
                    .WithMany(p => p.AddOnLabels)
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnLabel_AddOn");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AddOnLabelCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnLabel_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.AddOnLabelModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AddOnLabel_EmployeeModified");
            });

            modelBuilder.Entity<AddOnPrice>(entity =>
            {
                entity.ToTable("AddOnPrice", "Admin");

                entity.HasIndex(e => new { e.AddOnId, e.CountryCurrencyId, e.PriceLevelId })
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ForeverNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ForeverPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ForeverPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.AddOn)
                    .WithMany(p => p.AddOnPrices)
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnPrice_AddOn");

                entity.HasOne(d => d.CountryCurrency)
                    .WithMany(p => p.AddOnPrices)
                    .HasForeignKey(d => d.CountryCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnPrice_CountryCurrency");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AddOnPriceCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnPrice_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.AddOnPriceModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AddOnPrice_EmployeeModified");

                entity.HasOne(d => d.PriceLevel)
                    .WithMany(p => p.AddOnPrices)
                    .HasForeignKey(d => d.PriceLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnPrice_PriceLevel");
            });

            modelBuilder.Entity<AddOnSlider>(entity =>
            {
                entity.ToTable("AddOnSlider", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AddOn)
                    .WithMany(p => p.AddOnSliders)
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnSlider_AddOn");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AddOnSliderCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnSlider_EmployeeCreated");

                entity.HasOne(d => d.Media)
                    .WithMany(p => p.AddOnSliders)
                    .HasForeignKey(d => d.MediaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnSlider_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.AddOnSliderModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AddOnSlider_EmployeeModified");
            });

            modelBuilder.Entity<AddOnTag>(entity =>
            {
                entity.ToTable("AddOnTag", "Admin");

                entity.HasIndex(e => new { e.AddOnId, e.TagId })
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AddOn)
                    .WithMany(p => p.AddOnTags)
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnTag_AddOn");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AddOnTags)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnTag_Employee");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.AddOnTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddOnTag_Tag");
            });

            modelBuilder.Entity<AddonSubscription>(entity =>
            {
                entity.ToTable("AddonSubscription", "Customer");

                entity.Property(e => e.AddonName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AddonPrice)
                    .WithMany(p => p.AddonSubscriptions)
                    .HasForeignKey(d => d.AddonPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddonSubscription_AddOnPrice");

                entity.HasOne(d => d.CustomerSubscription)
                    .WithMany(p => p.AddonSubscriptions)
                    .HasForeignKey(d => d.CustomerSubscriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddonSubscription_CustomerSubscription");

                entity.HasOne(d => d.VersionSubscription)
                    .WithMany(p => p.AddonSubscriptions)
                    .HasForeignKey(d => d.VersionSubscriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddonSubscription_VersionSubscription");
            });

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Application", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LongDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.MainPageUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Application_EmployeeCreated");

                entity.HasOne(d => d.DeviceType)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.DeviceTypeId)
                    .HasConstraintName("FK__Applicati__Devic__69E9DAE2");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Application_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ApplicationModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Application_EmployeeModified");

                entity.HasOne(d => d.SubscriptionType)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.SubscriptionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Application_SubscriptionType");
            });

            modelBuilder.Entity<ApplicationLabel>(entity =>
            {
                entity.ToTable("ApplicationLabel", "Admin");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationLabels)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationLabel_Application");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationLabelCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationLabel_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ApplicationLabelModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ApplicationLabel_EmployeeModified");
            });

            modelBuilder.Entity<ApplicationSlider>(entity =>
            {
                entity.ToTable("ApplicationSlider", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationSliders)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationSlider_Application");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationSliderCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationSlider_EmployeeCreated");

                entity.HasOne(d => d.Media)
                    .WithMany(p => p.ApplicationSliders)
                    .HasForeignKey(d => d.MediaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationSlider_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ApplicationSliderModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ApplicationSlider_EmployeeModified");
            });

            modelBuilder.Entity<ApplicationTag>(entity =>
            {
                entity.ToTable("ApplicationTag", "Admin");

                entity.HasIndex(e => new { e.ApplicationId, e.TagId })
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationTags)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationTag_Application");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationTags)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationTag_Employee");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ApplicationTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationTag_Tag");
            });

            modelBuilder.Entity<AuditActionType>(entity =>
            {
                entity.ToTable("AuditActionType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ActionType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TempPrimaryKey).HasMaxLength(50);

                entity.HasOne(d => d.AuditActionType)
                    .WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.AuditActionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AuditLog_AuditActionType");
            });

            modelBuilder.Entity<AuditLogDetail>(entity =>
            {
                entity.ToTable("AuditLogDetail");

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NewValue).HasMaxLength(4000);

                entity.Property(e => e.OldValue).HasMaxLength(4000);

                entity.HasOne(d => d.AuditLog)
                    .WithMany(p => p.AuditLogDetails)
                    .HasForeignKey(d => d.AuditLogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AuditLogDetail_AuditLog");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessage", "Customer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message).HasMaxLength(500);

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.ReceivedTime).HasColumnType("datetime");

                entity.Property(e => e.SendTime).HasColumnType("datetime");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatMessage_Ticket");
            });

            modelBuilder.Entity<CompanySize>(entity =>
            {
                entity.ToTable("CompanySize", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Crmid).HasColumnName("CRMId");

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Connection>(entity =>
            {
                entity.ToTable("Connection", "Admin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnectionId).HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<ContactUsHelpOption>(entity =>
            {
                entity.ToTable("ContactUsHelpOption", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.HelpName)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract", "Customer");

                entity.HasIndex(e => e.Serial)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Serial)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Contract)
                    .HasForeignKey<Contract>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contract_Customer");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country", "LookUp");

                entity.Property(e => e.Crmid)
                    .HasMaxLength(500)
                    .HasColumnName("CRMId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.PhoneCode).HasMaxLength(5);
            });

            modelBuilder.Entity<CountryCurrency>(entity =>
            {
                entity.ToTable("CountryCurrency", "Admin");

                entity.HasIndex(e => e.CountryId)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Country)
                    .WithOne(p => p.CountryCurrency)
                    .HasForeignKey<CountryCurrency>(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountryCurrency_Country");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CountryCurrencyCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountryCurrency_Employee");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CountryCurrencies)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountryCurrency_Currency");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CountryCurrencyModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_CountryCurrency_Employee1");
            });

            modelBuilder.Entity<CountryPaymentMethod>(entity =>
            {
                entity.ToTable("CountryPaymentMethod", "Admin");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CountryPaymentMethods)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_CountryPaymentMethod_Country");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.CountryPaymentMethods)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountryPaymentMethod_PaymentMethod");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currency", "LookUp");

                entity.Property(e => e.Code).HasMaxLength(80);

                entity.Property(e => e.Crmid)
                    .HasMaxLength(500)
                    .HasColumnName("CRMId");

                entity.Property(e => e.MultiplyFactor).HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Symbole).HasMaxLength(10);
            });

            modelBuilder.Entity<CurrencyTable>(entity =>
            {
                entity.ToTable("CurrencyTable", "Dexef");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.ArName)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.ArNameCountry)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.ArNameSubName)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.DiallingCode).HasMaxLength(50);

                entity.Property(e => e.EnName)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.EnNameCountry)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.EnNameSubName)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(50)
                    .UseCollation("Arabic_CI_AS");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer", "Customer");

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.CompanyWebsite).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.CrmleadId)
                    .HasMaxLength(500)
                    .HasColumnName("CRMLeadId");

                entity.Property(e => e.CustomerCrmid)
                    .HasMaxLength(500)
                    .HasColumnName("CustomerCRMId");

                entity.Property(e => e.FullAddress).HasMaxLength(250);

                entity.Property(e => e.LastPasswordUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OauthResponse)
                    .HasMaxLength(4000)
                    .HasColumnName("OAuthResponse");

                entity.Property(e => e.OauthTypeId)
                    .HasColumnName("OAuthTypeId")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.PostalCode).HasMaxLength(50);

                entity.Property(e => e.TaxRegistrationNumber).HasMaxLength(50);

                entity.HasOne(d => d.CompanySize)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CompanySizeId)
                    .HasConstraintName("FK_Customer_CompanySize");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Country");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Customer_Employee");

                entity.HasOne(d => d.CustomerStatus)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CustomerStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_CustomerStatus");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Customer_FileStorage");

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.IndustryId)
                    .HasConstraintName("FK_Customer_Industry");

                entity.HasOne(d => d.OauthType)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.OauthTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_OAuthType");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.SourceId)
                    .HasConstraintName("FK_Customer_RegistrationSource");
            });

            modelBuilder.Entity<CustomerCard>(entity =>
            {
                entity.ToTable("CustomerCard", "Customer");

                entity.Property(e => e.CardNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CardToken)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ExtraInfo).HasMaxLength(4000);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCards)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCard_Customer");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.CustomerCards)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCard_PaymentMethod");
            });

            modelBuilder.Entity<CustomerEmail>(entity =>
            {
                entity.ToTable("CustomerEmail", "Customer");

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("([IsVerified]=(1))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerEmails)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerEmail_Customer");
            });

            modelBuilder.Entity<CustomerEmailVerification>(entity =>
            {
                entity.ToTable("CustomerEmailVerification", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.VerificationCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CustomerEmail)
                    .WithMany(p => p.CustomerEmailVerifications)
                    .HasForeignKey(d => d.CustomerEmailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerEmailVerification_CustomerEmail");
            });

            modelBuilder.Entity<CustomerMobile>(entity =>
            {
                entity.ToTable("CustomerMobile", "Customer");

                entity.HasIndex(e => e.Mobile)
                    .IsUnique()
                    .HasFilter("([IsVerified]=(1))");

                entity.Property(e => e.BlockForSendingSmsuntil).HasColumnName("BlockForSendingSMSUntil");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.PhoneCode).HasMaxLength(5);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerMobiles)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerMobile_Customer");

                entity.HasOne(d => d.PhoneCountry)
                    .WithMany(p => p.CustomerMobiles)
                    .HasForeignKey(d => d.PhoneCountryId)
                    .HasConstraintName("FK_CustomerMobile_Country");
            });

            modelBuilder.Entity<CustomerReview>(entity =>
            {
                entity.ToTable("CustomerReview", "Customer");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Review).HasMaxLength(4000);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.CustomerReviews)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerReview_Application");

                entity.HasOne(d => d.ApprovedByNavigation)
                    .WithMany(p => p.CustomerReviews)
                    .HasForeignKey(d => d.ApprovedBy)
                    .HasConstraintName("FK_CustomerReview_Employee");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerReviews)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerReview_Customer");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CustomerReviews)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerReview_CustomerReviewStatus");
            });

            modelBuilder.Entity<CustomerReviewStatu>(entity =>
            {
                entity.ToTable("CustomerReviewStatus", "LookUp");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CustomerSmsverification>(entity =>
            {
                entity.ToTable("CustomerSMSVerification", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.SmstypeId)
                    .HasColumnName("SMSTypeId")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VerificationCode)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.HasOne(d => d.CustomerMobile)
                    .WithMany(p => p.CustomerSmsverifications)
                    .HasForeignKey(d => d.CustomerMobileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerSMSVerification_CustomerMobile");
            });

            modelBuilder.Entity<CustomerStatu>(entity =>
            {
                entity.ToTable("CustomerStatus", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CustomerSubscription>(entity =>
            {
                entity.ToTable("CustomerSubscription", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PriceAfterDiscount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerSubscriptions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionSubscription_Customer");

                entity.HasOne(d => d.SubscriptionType)
                    .WithMany(p => p.CustomerSubscriptions)
                    .HasForeignKey(d => d.SubscriptionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerSubscription_SubscriptionType");
            });

            modelBuilder.Entity<DevicesType>(entity =>
            {
                entity.ToTable("DevicesType");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Device)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DexefBranch>(entity =>
            {
                entity.ToTable("DexefBranch", "LookUp");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("latitude");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone2).HasMaxLength(50);

                entity.Property(e => e.Phone3).HasMaxLength(50);

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.DexefBranches)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DexefBranch_Country");
            });

            modelBuilder.Entity<DownloadAddOnLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DownloadAddOnLog", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");

                entity.HasOne(d => d.AddOn)
                    .WithMany()
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DownloadAddOnLog_AddOn");

                entity.HasOne(d => d.Customer)
                    .WithMany()
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DownloadAddOnLog_Customer");
            });

            modelBuilder.Entity<DownloadVersionLog>(entity =>
            {
                entity.ToTable("DownloadVersionLog", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.DownloadVersionLogs)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_DownloadVersionLog_Customer");

                entity.HasOne(d => d.VersionIdRelease)
                    .WithMany(p => p.DownloadVersionLogs)
                    .HasForeignKey(d => d.VersionIdReleaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DownloadVersionLog_VersionRelease");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee", "Admin");

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.HasIndex(e => e.UserName)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IsAdmin).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsAdminForOtherCountries).HasDefaultValueSql("((0))");

                entity.Property(e => e.Mobile).HasMaxLength(25);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Employee_FileStorage");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Employee_Role");
            });

            modelBuilder.Entity<EmployeeCountry>(entity =>
            {
                entity.ToTable("EmployeeCountry", "Admin");

                entity.HasOne(d => d.CountryCurrency)
                    .WithMany(p => p.EmployeeCountries)
                    .HasForeignKey(d => d.CountryCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCountry_CountryCurrency");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCountries)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCountry_Employee");
            });

            modelBuilder.Entity<EmployeeType>(entity =>
            {
                entity.ToTable("EmployeeType", "LookUp");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.Property(e => e.Exception)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("Feature", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.FeatureCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feature_Employee");

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.Features)
                    .HasForeignKey(d => d.LogoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feature_File");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.FeatureModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Feature_EmployeeModified");
            });

            modelBuilder.Entity<FileStorage>(entity =>
            {
                entity.ToTable("FileStorage", "Admin");

                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.FileSize).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FullPath)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RealName).HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.FileStorages)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_FileStorage_Employee");

                entity.HasOne(d => d.FileType)
                    .WithMany(p => p.FileStorages)
                    .HasForeignKey(d => d.FileTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_File_FileType");
            });

            modelBuilder.Entity<FileType>(entity =>
            {
                entity.ToTable("FileType", "Admin");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<GeneratedNorFile>(entity =>
            {
                entity.ToTable("GeneratedNorFile");

                entity.Property(e => e.NorFile).HasColumnType("image");

                entity.HasOne(d => d.RequestActivationKey)
                    .WithMany(p => p.GeneratedNorFiles)
                    .HasForeignKey(d => d.RequestActivationKeyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GeneratedNorFile_RequestActivationKey");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Industry>(entity =>
            {
                entity.ToTable("Industry", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Crmid).HasColumnName("CRMId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice", "Customer");

                entity.Property(e => e.Address).HasMaxLength(2000);

                entity.Property(e => e.CancelReason).HasMaxLength(2000);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.InvoiceTitle).HasMaxLength(50);

                entity.Property(e => e.Notes).HasMaxLength(4000);

                entity.Property(e => e.PaymentInfo).HasMaxLength(4000);

                entity.Property(e => e.PaymentInfoSearch).HasMaxLength(4000);

                entity.Property(e => e.PaymentResponse).HasMaxLength(4000);

                entity.Property(e => e.Serial)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TaxReg).HasMaxLength(150);

                entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalDiscountAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalVatAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VatPercentage).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_Invoice_Currency");

                entity.HasOne(d => d.CustomerSubscription)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerSubscriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_CustomerSubscription");

                entity.HasOne(d => d.InvoiceStatus)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.InvoiceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_InvoiceStatus");

                entity.HasOne(d => d.InvoiceType)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.InvoiceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_InvoiceType");

                entity.HasOne(d => d.PaidByNavigation)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PaidBy)
                    .HasConstraintName("FK_Invoice_Employee");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_PaymentMethod");
            });

            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.ToTable("InvoiceDetail", "Customer");

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VatAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VersionName).HasMaxLength(2000);

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceDetails)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InvoiceDetail_Invoice");
            });

            modelBuilder.Entity<InvoiceStatu>(entity =>
            {
                entity.ToTable("InvoiceStatus", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<InvoiceType>(entity =>
            {
                entity.ToTable("InvoiceType", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Type).HasMaxLength(150);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName)
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<License>(entity =>
            {
                entity.ToTable("License", "Customer");

                entity.HasIndex(e => new { e.Serial, e.CustomerSubscriptionId })
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DeviceName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Serial)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.ActivationFile)
                    .WithMany(p => p.Licenses)
                    .HasForeignKey(d => d.ActivationFileId)
                    .HasConstraintName("FK_License_FileStorage");

                entity.HasOne(d => d.CustomerSubscription)
                    .WithMany(p => p.Licenses)
                    .HasForeignKey(d => d.CustomerSubscriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_License_VersionSubscription");

                entity.HasOne(d => d.LicenseStatus)
                    .WithMany(p => p.Licenses)
                    .HasForeignKey(d => d.LicenseStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_License_LicenseStatus");
            });

            modelBuilder.Entity<LicenseActionType>(entity =>
            {
                entity.ToTable("LicenseActionType", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<LicenseFile>(entity =>
            {
                entity.ToTable("LicenseFile", "Customer");

                entity.HasOne(d => d.ActivationFile)
                    .WithMany(p => p.LicenseFiles)
                    .HasForeignKey(d => d.ActivationFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LicenseRequestFile_FileStorage");

                entity.HasOne(d => d.GeneratedByNavigation)
                    .WithMany(p => p.LicenseFiles)
                    .HasForeignKey(d => d.GeneratedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LicenseFile_Employee");
            });

            modelBuilder.Entity<LicenseLog>(entity =>
            {
                entity.ToTable("LicenseLog", "Customer");

                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.LicenseLogs)
                    .HasForeignKey(d => d.ActionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LicenseLog_LicenseActionType");

                entity.HasOne(d => d.CreatedByAdminNavigation)
                    .WithMany(p => p.LicenseLogs)
                    .HasForeignKey(d => d.CreatedByAdmin)
                    .HasConstraintName("FK_LicenseLog_Employee");

                entity.HasOne(d => d.CreatedByCustomerNavigation)
                    .WithMany(p => p.LicenseLogs)
                    .HasForeignKey(d => d.CreatedByCustomer)
                    .HasConstraintName("FK_LicenseLog_Customer");

                entity.HasOne(d => d.License)
                    .WithMany(p => p.LicenseLogs)
                    .HasForeignKey(d => d.LicenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LicenseLog_License");

                entity.HasOne(d => d.NewStatus)
                    .WithMany(p => p.LicenseLogNewStatus)
                    .HasForeignKey(d => d.NewStatusId)
                    .HasConstraintName("FK_LicenseLog_LicenseNewStatus");

                entity.HasOne(d => d.OldStatus)
                    .WithMany(p => p.LicenseLogOldStatus)
                    .HasForeignKey(d => d.OldStatusId)
                    .HasConstraintName("FK_LicenseLog_LicenseOldStatus");
            });

            modelBuilder.Entity<LicenseStatu>(entity =>
            {
                entity.ToTable("LicenseStatus", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Module", "Admin");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.HasIndex(e => e.Title)
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LongDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.MainPageUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ModuleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Module_EmployeeCreated");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Modules)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Module_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ModuleModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Module_Employee");
            });

            modelBuilder.Entity<ModuleSlider>(entity =>
            {
                entity.ToTable("ModuleSlider", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ModuleSliderCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleSlider_EmployeeCreated");

                entity.HasOne(d => d.Media)
                    .WithMany(p => p.ModuleSliders)
                    .HasForeignKey(d => d.MediaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleSlider_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ModuleSliderModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ModuleSlider_Employee");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.ModuleSliders)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleSlider_Module");
            });

            modelBuilder.Entity<ModuleTag>(entity =>
            {
                entity.ToTable("ModuleTag", "Admin");

                entity.HasIndex(e => new { e.ModuleId, e.TagId })
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ModuleTags)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleTag_Employee");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.ModuleTags)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleTag_Module");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ModuleTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleTag_Tag");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification", "Admin");

                entity.Property(e => e.TicketId).HasMaxLength(150);

                entity.Property(e => e.TicketRefrences).HasMaxLength(225);

                entity.HasOne(d => d.CreatedByEmployee)
                    .WithMany(p => p.NotificationCreatedByEmployees)
                    .HasForeignKey(d => d.CreatedByEmployeeId)
                    .HasConstraintName("FK_Notification_EmployeeCreator");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Notification_Customer");

                entity.HasOne(d => d.HiddenByEmployee)
                    .WithMany(p => p.NotificationHiddenByEmployees)
                    .HasForeignKey(d => d.HiddenByEmployeeId)
                    .HasConstraintName("FK_Notification_EmployeeArchiver");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_Notification_Invoice");

                entity.HasOne(d => d.Licence)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.LicenceId)
                    .HasConstraintName("FK_Notification_License");

                entity.HasOne(d => d.NotificationAction)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotificationActionId)
                    .HasConstraintName("FK_Notification_NotificationAction");

                entity.HasOne(d => d.ReadByEmployee)
                    .WithMany(p => p.NotificationReadByEmployees)
                    .HasForeignKey(d => d.ReadByEmployeeId)
                    .HasConstraintName("FK_Notification_EmployeeReader");

                entity.HasOne(d => d.RefundRequest)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.RefundRequestId)
                    .HasConstraintName("FK_Notification_RefundRequest");
            });

            modelBuilder.Entity<NotificationAction>(entity =>
            {
                entity.ToTable("NotificationAction", "Admin");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(d => d.NotificationActionPage)
                    .WithMany(p => p.NotificationActions)
                    .HasForeignKey(d => d.NotificationActionPageId)
                    .HasConstraintName("FK_NotificationAction_NotificationActionPage");

                entity.HasOne(d => d.NotificationActionSubType)
                    .WithMany(p => p.NotificationActions)
                    .HasForeignKey(d => d.NotificationActionSubTypeId)
                    .HasConstraintName("FK_NotificationAction_NotificationActionSubType");

                entity.HasOne(d => d.NotificationActionType)
                    .WithMany(p => p.NotificationActions)
                    .HasForeignKey(d => d.NotificationActionTypeId)
                    .HasConstraintName("FK_NotificationAction_NotificationActionType");
            });

            modelBuilder.Entity<NotificationActionPage>(entity =>
            {
                entity.ToTable("NotificationActionPage", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<NotificationActionSubType>(entity =>
            {
                entity.ToTable("NotificationActionSubType", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.NotificationActionType)
                    .WithMany(p => p.NotificationActionSubTypes)
                    .HasForeignKey(d => d.NotificationActionTypeId)
                    .HasConstraintName("FK_NotificationActionSubType_NotificationActionType");
            });

            modelBuilder.Entity<NotificationActionType>(entity =>
            {
                entity.ToTable("NotificationActionType", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<OauthType>(entity =>
            {
                entity.ToTable("OAuthType", "Customer");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Page", "Admin");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<PageAction>(entity =>
            {
                entity.ToTable("PageAction", "Admin");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.PageActions)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageAction_Action");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PageActions)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageAction_Page");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Credential).HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.PaymentMethodCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_PaymentMethod_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.PaymentMethodModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_PaymentMethod_EmployeeModified");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.PaymentMethods)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMethod_PaymentType");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.ToTable("PaymentType", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<PriceLevel>(entity =>
            {
                entity.ToTable("PriceLevel", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NumberOfLicenses).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.PriceLevelCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceLevel_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.PriceLevelModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_PriceLevel_EmployeeModified");
            });

            modelBuilder.Entity<ReasonChangeDevice>(entity =>
            {
                entity.ToTable("ReasonChangeDevice", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Reason).HasMaxLength(150);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Jti)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasColumnName("JTI");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefreshToken_Customer");
            });

            modelBuilder.Entity<RefundRequest>(entity =>
            {
                entity.ToTable("RefundRequest", "Customer");

                entity.HasIndex(e => e.InvoiceId)
                    .IsUnique()
                    .HasFilter("([RefundRequestStatusId]<>(3))");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Invoice)
                    .WithOne(p => p.RefundRequest)
                    .HasForeignKey<RefundRequest>(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefundRequest_Invoice");

                entity.HasOne(d => d.RefundRequestStatus)
                    .WithMany(p => p.RefundRequests)
                    .HasForeignKey(d => d.RefundRequestStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefundRequest_RefundRequestStatus");
            });

            modelBuilder.Entity<RefundRequestStatu>(entity =>
            {
                entity.ToTable("RefundRequestStatus", "LookUp");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<RegistrationSource>(entity =>
            {
                entity.ToTable("RegistrationSource", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RequestActivationKey>(entity =>
            {
                entity.ToTable("RequestActivationKey", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.LicenseFile)
                    .WithMany(p => p.RequestActivationKeys)
                    .HasForeignKey(d => d.LicenseFileId)
                    .HasConstraintName("FK_RequestActivationKey_LicenseFile");

                entity.HasOne(d => d.License)
                    .WithMany(p => p.RequestActivationKeys)
                    .HasForeignKey(d => d.LicenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestActivationKey_License");

                entity.HasOne(d => d.RequestActivationKeyStatus)
                    .WithMany(p => p.RequestActivationKeys)
                    .HasForeignKey(d => d.RequestActivationKeyStatusId)
                    .HasConstraintName("FK_RequestActivationKey_RequestActivationKeyStatus");
            });

            modelBuilder.Entity<RequestActivationKeyStatu>(entity =>
            {
                entity.ToTable("RequestActivationKeyStatus", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<RequestChangeDevice>(entity =>
            {
                entity.ToTable("RequestChangeDevice", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.NewDeviceName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.NewSerial)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.OldDeviceName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.OldSerial)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.LicenseFile)
                    .WithMany(p => p.RequestChangeDevices)
                    .HasForeignKey(d => d.LicenseFileId)
                    .HasConstraintName("FK_RequestChangeDevice_LicenseFile");

                entity.HasOne(d => d.License)
                    .WithMany(p => p.RequestChangeDevices)
                    .HasForeignKey(d => d.LicenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestChangeDevice_License");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.RequestChangeDevices)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_RequestChangeDevice_Employee");

                entity.HasOne(d => d.ReasonChangeDevice)
                    .WithMany(p => p.RequestChangeDevices)
                    .HasForeignKey(d => d.ReasonChangeDeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestChangeDevice_ReasonChangeDevice");

                entity.HasOne(d => d.RequestChangeDeviceStatus)
                    .WithMany(p => p.RequestChangeDevices)
                    .HasForeignKey(d => d.RequestChangeDeviceStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestChangeDevice_RequestChangeDeviceStatus");
            });

            modelBuilder.Entity<RequestChangeDeviceStatu>(entity =>
            {
                entity.ToTable("RequestChangeDeviceStatus", "Customer");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.RoleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.RoleModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Role_EmployeeModified");
            });

            modelBuilder.Entity<RolePageAction>(entity =>
            {
                entity.ToTable("RolePageAction", "Admin");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.RolePageActionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePageAction_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.RolePageActionModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_RolePageAction_EmployeeModified");

                entity.HasOne(d => d.PageAction)
                    .WithMany(p => p.RolePageActions)
                    .HasForeignKey(d => d.PageActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePageAction_PageAction");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePageActions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePageAction_Role");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat);

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score });

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SimpleDatabas>(entity =>
            {
                entity.ToTable("SimpleDatabases", "LookUp");

                entity.Property(e => e.DataBaseName).HasMaxLength(150);

                entity.Property(e => e.FileName).HasMaxLength(50);

                entity.Property(e => e.FilePath).HasMaxLength(150);

                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<Smstype>(entity =>
            {
                entity.ToTable("SMSType", "Customer");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<SubscriptionType>(entity =>
            {
                entity.ToTable("SubscriptionType", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TableName>(entity =>
            {
                entity.ToTable("TableName", "LookUp");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TagCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tag_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.TagModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Tag_EmployeeModified");
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("Tax", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Taxes)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tax_Country");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TaxCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tax_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.TaxModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Tax_EmployeeModified");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Ticket", "Customer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ProductNumber).HasMaxLength(50);

                entity.Property(e => e.Program).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(400);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Ticket_Customer1");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Ticket_TicketStatus");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Ticket_Ticket_Types");
            });

            modelBuilder.Entity<TicketStatu>(entity =>
            {
                entity.ToTable("TicketStatus", "LookUp");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TicketType>(entity =>
            {
                entity.ToTable("Ticket_Types", "LookUp");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Version>(entity =>
            {
                entity.ToTable("Version", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LongDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.MainPageUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.ProductCrmId).HasMaxLength(500);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Versions)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Version_Application");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Version_EmployeeCreated");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Versions)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Version_FileStorage");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VersionModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Version_Employee");
            });

            modelBuilder.Entity<VersionAddon>(entity =>
            {
                entity.ToTable("VersionAddon", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MoreDetail).HasMaxLength(2000);

                entity.HasOne(d => d.Addon)
                    .WithMany(p => p.VersionAddons)
                    .HasForeignKey(d => d.AddonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionAddon_AddOn");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionAddonCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionAddon_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VersionAddonModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_VersionAddon_Employee");

                entity.HasOne(d => d.Version)
                    .WithMany(p => p.VersionAddons)
                    .HasForeignKey(d => d.VersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionAddon_Version");
            });

            modelBuilder.Entity<VersionFeature>(entity =>
            {
                entity.ToTable("VersionFeature", "Admin");

                entity.HasIndex(e => new { e.VersionId, e.FeatureId })
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MoreDetail).HasMaxLength(2000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionFeatureCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionFeature_EmployeeCreated");

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.VersionFeatures)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionFeature_Feature");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VersionFeatureModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_VersionFeature_Employee");

                entity.HasOne(d => d.Version)
                    .WithMany(p => p.VersionFeatures)
                    .HasForeignKey(d => d.VersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionFeature_Version");
            });

            modelBuilder.Entity<VersionModule>(entity =>
            {
                entity.ToTable("VersionModule", "Admin");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MoreDetail).HasMaxLength(2000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionModuleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionModule_Employee");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VersionModuleModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_VersionModule_EmployeeModified");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.VersionModules)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionModule_Module");

                entity.HasOne(d => d.Version)
                    .WithMany(p => p.VersionModules)
                    .HasForeignKey(d => d.VersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionModule_Version");
            });

            modelBuilder.Entity<VersionPrice>(entity =>
            {
                entity.ToTable("VersionPrice", "Admin");

                entity.HasIndex(e => new { e.VersionId, e.CountryCurrencyId, e.PriceLevelId })
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ForeverNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ForeverPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ForeverPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyNetPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyPrecentageDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.CountryCurrency)
                    .WithMany(p => p.VersionPrices)
                    .HasForeignKey(d => d.CountryCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionPrice_CountryCurrency");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionPriceCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionPrice_EmployeeCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VersionPriceModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_VersionPrice_Employee");

                entity.HasOne(d => d.PriceLevel)
                    .WithMany(p => p.VersionPrices)
                    .HasForeignKey(d => d.PriceLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionPrice_PriceLevel");

                entity.HasOne(d => d.Version)
                    .WithMany(p => p.VersionPrices)
                    .HasForeignKey(d => d.VersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionPrice_Version");
            });

            modelBuilder.Entity<VersionRelease>(entity =>
            {
                entity.ToTable("VersionRelease", "Admin");

                entity.HasIndex(e => new { e.VersionId, e.ReleaseNumber })
                    .IsUnique();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DownloadUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.ReleaseNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VersionReleases)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionRelease_Employee");

                entity.HasOne(d => d.Version)
                    .WithMany(p => p.VersionReleases)
                    .HasForeignKey(d => d.VersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionRelease_Version");
            });

            modelBuilder.Entity<VersionSubscription>(entity =>
            {
                entity.ToTable("VersionSubscription", "Customer");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.VersionName)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.CustomerSubscription)
                    .WithMany(p => p.VersionSubscriptions)
                    .HasForeignKey(d => d.CustomerSubscriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionSubscription_CustomerSubscription");

                entity.HasOne(d => d.VersionPrice)
                    .WithMany(p => p.VersionSubscriptions)
                    .HasForeignKey(d => d.VersionPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionSubscription_VersionPrice");

                entity.HasOne(d => d.VersionRelease)
                    .WithMany(p => p.VersionSubscriptions)
                    .HasForeignKey(d => d.VersionReleaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VersionSubscription_VersionRelease");
            });

            modelBuilder.Entity<ViewApplicationVersionFeature>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_ApplicationVersionFeatures", "Admin");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.FeatureName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.MoreDetail).HasMaxLength(2000);

                entity.Property(e => e.VersionName)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<ViewDashboardTotal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_DashboardTotals", "Admin");
            });

            modelBuilder.Entity<ViewLicenseRequest>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_LicenseRequests", "Admin");

                entity.Property(e => e.AddonName).HasMaxLength(500);

                entity.Property(e => e.AddonVersionName).HasMaxLength(4000);

                entity.Property(e => e.ContractSerial)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DeviceName).HasMaxLength(150);

                entity.Property(e => e.OldDevice).HasMaxLength(150);

                entity.Property(e => e.OldSerial).HasMaxLength(250);

                entity.Property(e => e.Reason).HasMaxLength(4000);

                entity.Property(e => e.RequestType)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Serial).HasMaxLength(250);

                entity.Property(e => e.VersionName).HasMaxLength(4000);
            });

            modelBuilder.Entity<ViewLicenseRequestsForValy>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_LicenseRequestsForValy");

                entity.Property(e => e.AddonName).HasMaxLength(4000);

                entity.Property(e => e.ApplicationName).HasMaxLength(4000);

                entity.Property(e => e.Serial)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.VersionName).HasMaxLength(4000);
            });

            modelBuilder.Entity<ViewMissingAddOnPrice>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_MissingAddOnPrices", "Admin");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CurrencyShortCode).HasMaxLength(10);

                entity.Property(e => e.PriceLevelName)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ViewMissingVersionPrice>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_MissingVersionPrices", "Admin");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CurrencyShortCode).HasMaxLength(10);

                entity.Property(e => e.PriceLevelName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.VersionName)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<WishListAddOn>(entity =>
            {
                entity.ToTable("WishListAddOn", "Customer");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AddOn)
                    .WithMany(p => p.WishListAddOns)
                    .HasForeignKey(d => d.AddOnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WishListAddOn_AddOn");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.WishListAddOns)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WishListAddOn_Customer");
            });

            modelBuilder.Entity<WishListApplication>(entity =>
            {
                entity.ToTable("WishListApplication", "Customer");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.WishListApplications)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WishListApplication_Application");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.WishListApplications)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WishListApplication_Customer");
            });

            modelBuilder.Entity<WorkSpaceStatu>(entity =>
            {
                entity.ToTable("WorkSpaceStatus", "LookUp");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Workspace>(entity =>
            {
                entity.ToTable("Workspace", "Customer");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.ConnectionString)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DatabaseName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.ServerIp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ServerIP");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Workspaces)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Workspace_Customer");

                entity.HasOne(d => d.DexefCountry)
                    .WithMany(p => p.WorkspaceDexefCountries)
                    .HasForeignKey(d => d.DexefCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Workspace_CurrencyTable");

                entity.HasOne(d => d.DexefCurrency)
                    .WithMany(p => p.WorkspaceDexefCurrencies)
                    .HasForeignKey(d => d.DexefCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Workspace_CurrencyTable1");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Workspaces)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Workspace_FileStorage");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Workspaces)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Workspace_WorkSpaceStatus");

                entity.HasOne(d => d.VersionSubscription)
                    .WithMany(p => p.Workspaces)
                    .HasForeignKey(d => d.VersionSubscriptionId)
                    .HasConstraintName("FK_Workspace_VersionSubscription");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
