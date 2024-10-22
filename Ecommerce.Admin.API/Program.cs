using Hangfire.Logging;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

using Ecommerce.BLL.Responses;

using Serilog;
using Serilog.Events;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ecommerce.API
{

    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static int Main( string [] args )
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");
                var host = CreateHostBuilder(args).Build();
                ServiceProviderFactory.ServiceProvider = (IServiceProvider)host.Services.GetService(typeof(IServiceProvider));
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder( string [] args ) =>
        Host.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseSerilog(( context, services, configuration ) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services))
                //.ConfigureLogging(())
                //.configureServices()
                .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseIIS();
                  webBuilder.UseStartup<Startup>();
                  webBuilder.UseDefaultServiceProvider(options =>
                      options.ValidateScopes = false);
                  webBuilder.ConfigureAppConfiguration(( hostBuilderContext, build ) =>
                   {
                       hostBuilderContext.HostingEnvironment.EnvironmentName = (args != null && args.Count() > 0) ? args [0] : hostBuilderContext.HostingEnvironment.EnvironmentName;
                       var hostEnviroment = hostBuilderContext.HostingEnvironment;
                       var env = hostEnviroment.EnvironmentName;
                       Console.WriteLine(env);
                       build.SetBasePath(Directory.GetCurrentDirectory());
                       build.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                       build.AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true);
                       //build.AddJsonFile($"appsettings.overrides.json", optional: true, reloadOnChange: true);

                       build.AddEnvironmentVariables(); // overwrites previous values
                       if (args != null)
                       {
                           build.AddCommandLine(args);
                       }
                   });
              })
            ;

    }
}
