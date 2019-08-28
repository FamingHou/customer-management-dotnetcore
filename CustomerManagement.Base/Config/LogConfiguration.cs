using NLog;
using NLog.Web;
using System;
using System.IO;

namespace CustomerManagement.Base.Config
{
    public static class LogConfiguration
    {
        public static Logger Configure()
        {
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = $"{basePath}/nlog.config";
            var aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentSpecificFilePath = $"{basePath}/nlog.{aspnetEnvironment}.config";
            if (File.Exists(environmentSpecificFilePath))
                filePath = environmentSpecificFilePath;
            return NLogBuilder.ConfigureNLog(filePath).GetCurrentClassLogger();
        }
    }
}