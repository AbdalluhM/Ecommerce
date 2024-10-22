using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API
{
    public class Program
    {
        public static void Main( string [] args )
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder( string [] args ) =>
            Host.CreateDefaultBuilder(args)
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .UseDefaultServiceProvider(options =>
                  options.ValidateScopes = false)
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
                        build.AddEnvironmentVariables(); // overwrites previous values
                        if (args != null)
                        {
                            build.AddCommandLine(args);
                        }
                    });
                });
    }
}
