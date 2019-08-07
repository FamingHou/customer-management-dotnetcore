# customer-management-dotnetcore

## NLog

### Configuration

#### nlog.config & nlog.Development.config

Create `nlog.config` and `nlog.Development.config` in `config` directory

```
<?xml version="1.0" encoding="utf-8"?>

<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">

  <extensions>
    <add assembly="NLog.Web.AspNetCore" />
  </extensions>

  <targets>
    <target name="console" xsi:type="Console">
      <layout xsi:type="JsonLayout" includeAllProperties="true">
        <attribute name="format" layout="2" />
        <attribute name="service" layout="CustomerManagement-Service" />
        <attribute name="timestamp" layout="${ticks}" />
        <attribute name="timestampISO" layout="${longdate}" />
        <attribute name="logger" layout="${logger}" />
        <attribute name="threadid" layout="${threadid}" />
        <attribute name="levelTag" layout="${level:upperCase=true}" />
        <attribute name="message" layout="${message}" />
        <attribute name="exceptionType" layout="${exception:format=Type}" />
        <attribute name="exceptionMessage" layout="${exception:format=Message}" />
        <attribute name="exceptionStackTrace" layout="${exception:format=StackTrace}" />
        <attribute name="environment" layout="${environment:ASPNETCORE_ENVIRONMENT}" />
      </layout>
    </target>
    <target name="file" xsi:type="File"
            layout="${longdate}:${logger}:${threadid}:${level:uppercase=true}:${message}"
            fileName="${basedir}/logs/nlog-${shortdate}.log"
            maxArchiveFiles="5"
            archiveAboveSize="10240" />
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="console" />
    <logger name="*" minlevel="Trace" writeTo="file" />
  </rules>
</nlog>
```

### Link 

```
  <ItemGroup>
    <None Include="..\config\nlog.config" Link="nlog.config">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="..\config\nlog.Development.config" Link="nlog.Development.config">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

### Initialization

```
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
```

### Logging

```
namespace CustomerManagement.Api.Web.Controllers
{
    ...
    public class ValuesController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        ...
        public ActionResult<IEnumerable<string>> Get()
        {
            Logger.Debug("This is a log message");
            ...
        }
```

## Docker Compose

In the `CustomerManagement.Api.Web` project, right-click on the project node, and choose **Add** > **Container Orchestrator Support**. Choose **Docker Compose**, and then select **Linux**.

Visual Studio creates the docker compose YML file.

```yaml
version: '3.4'

services:
  customermanagement.api.web:
    image: ${DOCKER_REGISTRY-}customermanagementapiweb
    build:
      context: .
      dockerfile: CustomerManagement.Api.Web/Dockerfile

```