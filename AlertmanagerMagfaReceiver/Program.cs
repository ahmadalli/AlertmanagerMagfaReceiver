using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlertmanagerMagfaReceiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
                {
                    configurationBuilder.AddEnvironmentVariables("APP_");
                })
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    if (hostContext.Configuration.GetValue("Logging.Console.Enabled", false))
                    {
                        loggingBuilder.AddConsole();
                    }

                    if (hostContext.Configuration.GetValue("Logging.Sentry.Enabled", false))
                    {
                        loggingBuilder.AddSentry(c =>
                        {
                            c.Dsn = hostContext.Configuration.GetValue("Logging.Sentry.DSN", "");
                            c.AttachStacktrace = true;
                        });
                    }

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
