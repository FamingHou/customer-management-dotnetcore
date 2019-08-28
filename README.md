# customer-management-dotnetcore

## Overview

![Tour features](docs/overview.gif)

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

#### Package Reference

CustomerManagement.Storage.SqlServerAdapter/CustomerManagement.Storage.SqlServerAdapter.csproj

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="2.1.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="2.1.11" />
  </ItemGroup>
```


### Define entity classes

CustomerManagement.Storage.SqlServerAdapter/Entity/CustomerEntity.cs
```csharp
namespace CustomerManagement.Storage.SqlServerAdapter.Entity
{
    [Table("Customers")]
    public class CustomerEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<CommentEntity> Comments { get; set; }
    }
}
```

CustomerManagement.Storage.SqlServerAdapter/Entity/CommentEntity.cs
```csharp
namespace CustomerManagement.Storage.SqlServerAdapter.Entity
{
    [Table("Comments")]
    public class CommentEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public virtual CustomerEntity Customer { get; set; }
    }
}
```

### Define a context class

CustomerManagement.Storage.SqlServerAdapter/Context/CustomerDbContext.cs
```csharp
namespace CustomerManagement.Storage.SqlServerAdapter.Context
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        ...
    }
}
```

### Register CustomerDbContext with dependency injection

CustomerManagement.DataService/Startup/StorageConfiguration.cs
```csharp
        private static void RegisterSqlServerDbContext(IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(dbConnectionString));
        }
```

### Define the interface ICustomerService in Data Service layer (Database, Elastic Search etc...)

CustomerManagement.DataService/Services/ICustomerService.cs

```csharp
namespace CustomerManagement.DataService.Services
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> GetById(Guid id);
    }
}
```

### Implement ICustomerService

CustomerManagement.DataService/Services/CustomerService.cs
```csharp
namespace CustomerManagement.DataService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDbStorage _customerDbStorage;
        ...
        public Task<Customer> CreateCustomer(Customer customer)
        {
            return _customerDbStorage.CreateCustomer(customer);
        }
        public Task<Customer> GetById(Guid id)
        {
            return _customerDbStorage.GetById(id);
        }
    }
}
```

### Define the interface ICustomerDbStorage (Database Only)

CustomerManagement.Base/Services/ICustomerDbStorage.cs

```csharp
namespace CustomerManagement.Base.Services
{
    public interface ICustomerDbStorage
    {
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> GetById(Guid id);
    }
}
```

### Implement the interface in Sql Server Adapter layer

CustomerManagement.Storage.SqlServerAdapter/Services/CustomerSqlServerStorage.cs

```csharp
namespace CustomerManagement.Storage.SqlServerAdapter.Services
{
    public class CustomerSqlServerStorage : ICustomerDbStorage
    {
        private readonly CustomerDbContext _context;
        ...
        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var customerEntity = ConvertToEntity(customer);
            var savedEntity = await _context.Customers.AddAsync(customerEntity);
            await _context.SaveChangesAsync();

            var returnedCustomer = ConvertToModel(savedEntity.Entity);
            return returnedCustomer;
        }

        public async Task<Customer> GetById(Guid id)
        {
            var customerEntity = await _context.Customers.AsNoTracking()
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync();
            return customerEntity != null ? ConvertToModel(customerEntity) : null;
        }
        ...
    }
}
```

### Register CustomerSqlServerStorage with dependency injection

CustomerManagement.DataService/Startup/StorageConfiguration.cs
```csharp
        private static void RegisterDatabaseServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerDbStorage, CustomerSqlServerStorage>();
        }
```

### Create a migration

Execute the command as below under directory *CustomerManagement.Storage.SqlServerAdapter*
```console
dotnet ef migrations add InitialCreate
```

```console
PS C:\Users\frank\Documents\Github\customer-management-dotnetcore\CustomerManagement.Storage.SqlServerAdapter> dotnet ef migrations add InitialCreate
Done. To undo this action, use 'ef migrations remove'
```

### Update the database

```console
dotnet ef database update
```

```console
PS C:\Users\frank\Documents\Github\customer-management-dotnetcore\CustomerManagement.Storage.SqlServerAdapter> dotnet ef database update
Applying migration '20190827220606_InitialCreate'.
Done.
```

## References

[Quickstart: Run SQL Server container images with Docker](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017&pivots=cs1-powershell)  
[Configure SQL Server container images on Docker](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker?view=sql-server-2017)  
[Quickstart: Compose and ASP.NET Core with SQL Server](https://docs.docker.com/compose/aspnet-mssql-compose/)  
[Getting Started with EF Core on ASP.NET Core with a New database](https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=visual-studio)  
[Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
