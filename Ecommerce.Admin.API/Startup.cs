using AutoMapper;
using Dexef.Notification.Managers;
using Dexef.Storage;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ecommerce.API.Attributes;
using Ecommerce.API.Middlewares;
using Ecommerce.BLL;
using Ecommerce.BLL.Customers.Invoices.Job;
using Ecommerce.BLL.Mapping;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Enums.Settings;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.DTO.Settings.Crm;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.DTO.Settings.Enviroment;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Settings.Notifications;
using Ecommerce.DTO.Settings.Notifications.Mails;
using Ecommerce.DTO.Settings.RealtimeNotifications;
using Ecommerce.DTO.Settings.StaticNumbers;
using Ecommerce.Helper.Security;
using Ecommerce.Localization;
using Ecommerce.Localization.Resources;
using Ecommerce.Reports;
using Ecommerce.Repositroy.Base;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ecommerce.API
{
    public class Startup
    {
        private const string _connStringName = "EcommerceConnection";
        private const string _corsPolicy = "CorsPolicy";


        private IList<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo(DXConstants.SupportedLanguage.EN),
                    new CultureInfo(DXConstants.SupportedLanguage.AR),
                };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.ConfigureReportingServices(configurator =>
            //{
            //    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
            //    {
            //        viewerConfigurator.UseCachedReportSourceBuilder();
            //    });
            //});

            #region AddApplicationInsights
            //services.AddApplicationInsightsTelemetry();
            #endregion

            #region Logging
            services.AddSingleton<RequestResponseLoggingMiddleware>();
            #endregion

            services.Configure<CrmSetting>(Configuration.GetSection(AppsettingsEnum.CrmSetting.ToString()));
            services.Configure<AuthSetting>(Configuration.GetSection(AppsettingsEnum.AuthSetting.ToString()));
            services.Configure<Numbers>(Configuration.GetSection(AppsettingsEnum.Numbers.ToString()));
            services.Configure<CompanyInfoPdf>(Configuration.GetSection(AppsettingsEnum.CompanyInfoPdf.ToString()));

            services.Configure<EmailTemplateSetting>(Configuration.GetSection(AppsettingsEnum.EmailTemplateSetting.ToString()));
            services.Configure<InvoiceSetting>(Configuration.GetSection(AppsettingsEnum.InvoiceSetting.ToString()));
            services.Configure<RealtimeNotificationSetting>(Configuration.GetSection(AppsettingsEnum.RealtimeNotificationSetting.ToString()));

            #region Localization

            services.AddRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(DXConstants.SupportedLanguage.EN);
                x.ApplyCurrentCultureToResponseHeaders = true;
                x.SupportedCultures = supportedCultures;
                x.SupportedUICultures = supportedCultures;
            });
            //services.AddLocalization(opt => opt.ResourcesPath = "Resources");
            // services.AddTransient<ICustomStringLocalizer, CustomStringLocalizer>();
            //services.AddTransient<ICustomStringLocalizer>
            //    (
            //    x => new CustomStringLocalizer<T>)
            //    );
            services.AddTransient(typeof(ICustomStringLocalizer<>), typeof(CustomStringLocalizer<>));



            #endregion


            //         services.AddControllers().AddJsonOptions(x =>
            //x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            #region Swagger 
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ecommerce Admin API",
                    Description = "ASP.NET Core 5 Web API"
                });

                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
            #endregion
            #region BLL
            foreach (var implementationType in typeof(BaseBLL).Assembly.GetTypes().Where(s => s.IsClass && s.Name.EndsWith("BLL") && !s.IsAbstract))
            {
                foreach (var interfaceType in implementationType.GetInterfaces())
                {
                    services.AddScoped(interfaceType, implementationType);
                }
            }
            #endregion

            #region DbFactory & UnitOfWork
            //services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString(_connStringName)));
            //services.AddDbContext<EcommerceContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString(_connStringName)));


            services.AddScoped<IDbFactory, DbFactory>(s => new DbFactory(new SqlConnection(Configuration.GetConnectionString(("EcommerceConnection")))));
            //services.AddScoped<IDbFactory, DbFactory>();
            //services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(Configuration.GetConnectionString(("EcommerceConnection"))));
            //DbContextOptions<EcommerceContext> dbContextOptions = new DbContextOptionsBuilder<EcommerceContext>()
            //      .Options;
            ////services.AddScoped<IDbFactory, DbFactory>();
            //services.AddScoped<IDbFactory, DbFactory>(s => new DbFactory(new ApplicationDbContext(dbContextOptions)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion
            services.AddSingleton<IReportManager, ReportManager>();
            #region Mapper
            //to inject mapping profiles,member value resolvers ,Type Resolvers 
            //don't upgrade AutoMapper.Extensions.Microsoft.DependencyInjection above version 7
            services.AddAutoMapper(typeof(EntityToDtoMappingProfile), typeof(DtoToEntityMappingProfile));
            //this is the old registeration methor
            //services.AddScoped(provider => new MapperConfiguration(cfg =>
            //{

            //    cfg.AddProfile(new EntityToDtoMappingProfile());
            //    cfg.AddProfile(new DtoToEntityMappingProfile());

            //   });
            //}).CreateMapper())
            //;
            //services.AddAutoMapper(typeof(ApplicationMissionPricesCountResolver));

            #endregion
            #region Repository
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            #endregion
            #region Helpers
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            #endregion
            #region CORS

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            #endregion
            #region Authentication
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var key = Encoding.ASCII.GetBytes(Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Secret)}"]);
                    var issuer = Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Issuer)}"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuer = issuer,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero// remove delay of token when expire
                   
                    };
                });
            services.AddScoped<DxAuthorizeAttribute>();
            //services.AddControllers(options => options.Filters.Add(typeof(DxAuthorizationAttribute))); 

            //services.AddControllersWithViews();

            #endregion

            #region Hangfire.
            services.AddHangfire(h => h.UseSqlServerStorage(Configuration.GetConnectionString((_connStringName))));
            services.AddHangfireServer();
            #endregion


            #region Payment GateWay Settings
            services.Configure<PaymentGateWaySettings>(Configuration.GetSection(AppsettingsEnum.PaymentGateWaySettings.ToString()));
            #endregion
            services.Configure<FileStorageSetting>(Configuration.GetSection(AppsettingsEnum.FileStorageSetting.ToString()));
            services.Configure<EnviromentSetting>(Configuration.GetSection(AppsettingsEnum.EnviromentSetting.ToString()));

            services.Configure<NotificationConfig>(Configuration.GetSection(AppsettingsEnum.NotificationConfig.ToString()));

            services.AddScoped<IDexefStorageManager, DexefStorageManager>();

            ServiceProviderFactory.ServiceCollection = services;

            services.AddDexefNotification();

            services.AddHttpClient();

            //services.AddDevExpressControls();
            services.AddMvcCore();

            //services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
            #region SignalR
            services.AddSignalR();

            services.AddScoped<IDexefHub, DexefHub>();

            #endregion
            DevExpress.Utils.AzureCompatibility.Enable = true;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInvoiceJobBLL invoiceJobBLL, IOptions<RealtimeNotificationSetting> realtimeNotifyOptions)
        {
            var realtimeNotifySetting = realtimeNotifyOptions.Value;

            app.UseSerilogRequestLogging();


            if (env.IsDevelopment() || env.IsStaging() || env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce.Admin API V1");
                });
                app.UseDeveloperExceptionPage();
            }

            #region JWT
            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();
            #endregion

            #region Localization
            app.UseMiddleware<LocalizationMiddleware>();
            #endregion

            #region Logging
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            #endregion

            app.UseRouting();

            app.UseCors(_corsPolicy);
            app.UseStaticFiles();
            //app.UseDevExpressControls();

            app.UseAuthentication();
            app.UseAuthorization();

            // Todo-Sully: remove dashboard in production.
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>()
                {
                    new PassThroughDashboardAuthorizationFilter()
                },
                IsReadOnlyFunc = context => false
            });

            RecurringJob.AddOrUpdate(() => invoiceJobBLL.DeleteOldGeneratedInoivcesPdf(), Cron.Daily(12));




            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DexefHub>($"/{realtimeNotifySetting.Hub}");
            });


        }
        //protected override void RegisterDependencies( ContainerBuilder containerBuilder )
        //{
        //    var mapperConfiguration = new MapperConfiguration(configuration =>
        //    {
        //        configuration.ConstructServicesUsing(componentContext.Resolve);
        //        configuration.AddProfiles(assemblies.Select(assembly => assembly.FullName));
        //    });

        //    containerBuilder.RegisterInstance(mapperConfiguration).As<MapperConfiguration>().SingleInstance();

        //    var valueResolvers = assemblies.SelectMany(
        //        assembly => assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsClosedTypeOf(typeof(IMemberValueResolver<,,,>)))
        //    );

        //    foreach (var valueResolver in valueResolvers)
        //    {
        //        containerBuilder.RegisterType(valueResolver)
        //            .AsSelf()
        //            .AsImplementedInterfaces();
        //    }
        //}
    }


}
