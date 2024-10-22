using AutoMapper;
using Dexef.Notification.Managers;
using Dexef.Storage;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ecommerce.BLL;
using Ecommerce.BLL.Chats;
using Ecommerce.BLL.Customers.Invoices.Job;
using Ecommerce.BLL.Mapping;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.Core.Enums.Settings;
using Ecommerce.Core.Infrastructure;
using Ecommerce.Customer.API.Middlewares;
using Ecommerce.Customer.API.Requirements;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.DTO.Settings.Crm;
using Ecommerce.DTO.Settings.CustomerSubscriptions;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.DTO.Settings.Enviroment;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Settings.Notifications;
using Ecommerce.DTO.Settings.Notifications.Mails;
using Ecommerce.DTO.Settings.Pdfs.PdfSettings;
using Ecommerce.DTO.Settings.RealtimeNotifications;
using Ecommerce.DTO.Settings.StaticNumbers;
using Ecommerce.Helper.Security;
using Ecommerce.Localization;
using Ecommerce.Localization.Resources;
using Ecommerce.Reports;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API
{
    //comment test 
    public class Startup
    {
        private const string _corsPolicy = "CorsPolicy";
        private const string _connStringName = "EcommerceConnection";
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

            // Register the in-memory cache services
            services.AddMemoryCache();


            services.AddControllers();

            #region Helpers
            services.AddSingleton<IPasswordHasher, PasswordHasher>();


            services.Configure<FileStorageSetting>(Configuration.GetSection(AppsettingsEnum.FileStorageSetting.ToString()));
            services.Configure<EnviromentSetting>(Configuration.GetSection(AppsettingsEnum.EnviromentSetting.ToString()));
            services.Configure<AuthSetting>(Configuration.GetSection(AppsettingsEnum.AuthSetting.ToString()));
            services.Configure<CrmSetting>(Configuration.GetSection(AppsettingsEnum.CrmSetting.ToString()));
            services.Configure<InvoicePdfSetting>(Configuration.GetSection(AppsettingsEnum.InvoicePdfSetting.ToString()));
            services.Configure<SubscriptionSetting>(Configuration.GetSection(AppsettingsEnum.SubscriptionSetting.ToString()));
            services.Configure<Numbers>(Configuration.GetSection(AppsettingsEnum.Numbers.ToString()));

            services.Configure<EmailTemplateSetting>(Configuration.GetSection(AppsettingsEnum.EmailTemplateSetting.ToString()));
            services.Configure<InvoiceSetting>(Configuration.GetSection(AppsettingsEnum.InvoiceSetting.ToString()));
            services.Configure<CompanyInfoPdf>(Configuration.GetSection(AppsettingsEnum.CompanyInfoPdf.ToString()));
            services.Configure<RealtimeNotificationSetting>(Configuration.GetSection(AppsettingsEnum.RealtimeNotificationSetting.ToString()));

            services.AddHttpClient();
            #endregion

            #region AddApplicationInsights
            //services.AddApplicationInsightsTelemetry();
            #endregion

            #region Logging
            services.AddSingleton<RequestResponseLoggingMiddleware>();
            #endregion

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


            #region HttpContextAccessor
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            #endregion

            #region Swagger 
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ecommerce Customers API",
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

            #region Notification Service
            services.Configure<NotificationConfig>(Configuration.GetSection(AppsettingsEnum.NotificationConfig.ToString()));
            services.Configure<ContactUsConfig>(Configuration.GetSection(AppsettingsEnum.ContactUsConfig.ToString()));

            ServiceProviderFactory.ServiceCollection = services;

            services.AddDexefNotification();

            services.AddTransient(typeof(ICustomStringLocalizer<>), typeof(CustomStringLocalizer<>));
            #endregion

            #region DbFactory & UnitOfWork
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString(_connStringName)));
            //services.AddDbContext<EcommerceContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString(_connStringName)));
            //services.AddScoped<IDbFactory, DbFactory>();

            services.AddScoped<IDbFactory, DbFactory>(s => new DbFactory(new SqlConnection(Configuration.GetConnectionString((_connStringName)))));
            services.AddSingleton<IReportManager, ReportManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region Mapper
            services.AddAutoMapper(typeof(EntityToDtoMappingProfile), typeof(DtoToEntityMappingProfile));
            #endregion

            #region Repository
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
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
            services.AddScoped<IChatHub, ChatHub>();

            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("SignalRPolicy", builder => builder.WithOrigins("localhost", "https://dexefcustomer.dexef.com/").AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed((host) => true).AllowCredentials());
                options.AddPolicy(_corsPolicy, builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            #endregion

            #region Authentication
            var key = Encoding.ASCII.GetBytes(Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Secret)}"]);
            var issuer = Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Issuer)}"];

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero // remove delay of token when expire
            };

            services.AddSingleton(tokenValidationParams);

            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(jwt =>
                    {
                        jwt.SaveToken = true;
                        jwt.TokenValidationParameters = tokenValidationParams;
                        jwt.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/chatHub")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConst.SuspendedUserNotAllowed,
                    policy => policy.Requirements.Add(new SuspendedUserRequirement()));
            });
            #endregion


            #region Hangfire.
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString(_connStringName), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            //services
            //    .AddAuthentication(options =>
            //    {
            //        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //    })
            //    .AddCookie()
            //    .AddGoogle(options =>
            //    {
            //        //IConfigurationSection googleAuthSection = Configuration.GetSection("Authentification:Google");
            //        options.ClientId = "471054911197-g8raudhub9ni0ih82c4l2javn4ftnuqc.apps.googleusercontent.com";
            //        options.ClientSecret = "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";
            //        options.UserInformationEndpoint.ToList();
            //        options.SaveTokens = true;
            //    });

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            //services.AddHangfire(h => h.UseSqlServerStorage(Configuration.GetConnectionString((_connStringName))));
            //services.AddHangfireServer();
            #endregion


            #region Payment GateWay Settings
            services.Configure<PaymentGateWaySettings>(Configuration.GetSection(AppsettingsEnum.PaymentGateWaySettings.ToString()));
            #endregion

            #region Pdf (DinkToPdf)
            // DinkToPdf: generate pdf library configuration.

            // DinkToPdf: generate pdf library configuration.

            //var wkHtmlToPdfContext = new CustomAssemblyLoadContext();
            //var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            //var wkHtmlToPdfPath = Path.Combine(AppContext.BaseDirectory, $"wkhtmltox\\{architectureFolder}\\libwkhtmltox");
            //wkHtmlToPdfContext.LoadUnmanagedLibrary(wkHtmlToPdfPath);

            //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            #endregion

            services.AddHttpClient();
            services.AddMemoryCache();
            //services.AddDevExpressControls();
            services.AddMvcCore();
            #region SignalR

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.HandshakeTimeout = TimeSpan.FromSeconds(10);
                o.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
                o.KeepAliveInterval = TimeSpan.FromMinutes(2);
                o.MaximumReceiveMessageSize = null;
            });
            services.AddScoped<IDexefHub, DexefHub>();
            #endregion

            services.AddScoped<IDexefStorageManager, DexefStorageManager>();

            DevExpress.Utils.AzureCompatibility.Enable = true;

            // services.AddRazorPages();
            //services.AddControllersWithViews(mvcOptions => mvcOptions.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInvoiceJobBLL invoiceJobBLL)
        {
            if (env.IsDevelopment() || env.IsStaging() || env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce.Customer API V1");
                });
                app.UseDeveloperExceptionPage();
            }
            app.UseRequestLimiterMiddleware(requestLimit: 100, timeWindow: TimeSpan.FromMinutes(10));

            #region Localization 
            #region Logging
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            #endregion Localization
            app.UseMiddleware<LocalizationMiddleware>();
            #endregion

            app.UseRouting();
            app.UseCors("SignalRPolicy");
            app.UseCors(_corsPolicy);
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMiddleware<DuplicateRequestMiddleware>();
            app.UseAuthorization();

            // Todo-Sully: remove dashboard in production.
            //if (!env.IsProduction())
            //{
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>()
                    {
                        new PassThroughDashboardAuthorizationFilter()
                    },
                IsReadOnlyFunc = context => false
            });
            ////}



            RecurringJob.AddOrUpdate(() => invoiceJobBLL.CheckInvoicesExpireationAutoAsync(), Cron.Daily(12));
            RecurringJob.AddOrUpdate(() => invoiceJobBLL.DeleteOldGeneratedInoivcesPdf(), Cron.Daily(1));
            RecurringJob.AddOrUpdate(() => invoiceJobBLL.UpdateFawryReferenceInvoices(), Cron.Daily(2));
            RecurringJob.AddOrUpdate(() => invoiceJobBLL.BlockWorkSpaceAfterTrialPeriod(), Cron.Daily(12));
            ////TODO:Comment this after inserting CRM Old Invoices
            //RecurringJob.AddOrUpdate(( ) => invoiceJobBLL.RenewOldInvoicesAutoAsync(), Cron.Daily(3));

            ////Check duplicate invoices 
            RecurringJob.AddOrUpdate(() => invoiceJobBLL.RenewInvoicesAutoAsync(), Cron.Daily(4));



            app.UseWebSockets();
            //app.UseMvc(routes =>
            //{
            // You can add all the routes you need here
            // And the default route :
            //    routes.MapRoute(
            //         name: "default_route",
            //         template: "{controller}/{action}/{id?}",
            //         defaults: new { controller = "Home", action = "Index" }
            //    );
            //});
            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapRazorPages();

                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub"); // Map SignalR hub

            });
        }
    }
}
