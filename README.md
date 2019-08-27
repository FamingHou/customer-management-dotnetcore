# customer-management-dotnetcore

## NLog

### Configuration

#### nlog.config & nlog.Development.config

Create `nlog.config` and `nlog.Development.config` in `config` directory

`./config/nlog.config`
```xml
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
        ...
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

`./CustomerManagement.Api.Web/CustomerManagement.Api.Web.csproj`
```xml
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

`./CustomerManagement.Base/Configuration/LogConfiguration.cs`
```csharp
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

`./CustomerManagement.Api.Web/Controllers/ValuesController.cs`

```csharp
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

## Database

### SQL Server

Add `sqlserver` service in `docker-compose.yml`

```yml
  db:
    image: microsoft/mssql-server-linux:2017-latest
    
    ports:
      - "1433:1433"
    environment:
      - SA_PASSWORD=Pass@word
      - ACCEPT_EULA=Y
```

Run the SQL Server container

```
docker-compose up db
```

[Connect from outside the container](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017&pivots=cs1-powershell#connectexternal)

```
sqlcmd -S <ip_address>,5434 -U SA -P "Pass@word"
```

## Entity Framework Core

### Install EF Core



## References

[Quickstart: Run SQL Server container images with Docker](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017&pivots=cs1-powershell)  
[Configure SQL Server container images on Docker](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker?view=sql-server-2017)  
[Quickstart: Compose and ASP.NET Core with SQL Server](https://docs.docker.com/compose/aspnet-mssql-compose/)  
[Getting Started with EF Core on ASP.NET Core with a New database](https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=visual-studio)  
