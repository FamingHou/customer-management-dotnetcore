using CustomerManagement.Base.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;
using System;

namespace CustomerManagement.Api.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogConfiguration.Configure();
            try
            {
                logger.Debug("Init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}