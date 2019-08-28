using CustomerManagement.Base.Services;
using CustomerManagement.Storage.SqlServerAdapter.Context;
using CustomerManagement.Storage.SqlServerAdapter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using CustomerManagement.DataService.Services;
using CustomerManagement.Storage.SqlServerAdapter.Startup;
using Microsoft.Extensions.Configuration;

namespace CustomerManagement.DataService.Startup
{
    public static class StorageConfiguration
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            RegisterCustomerService(services);
            //RegisterSqlServerDbContext(services, @"Server=192.168.1.6;Database=slave;User=SA;Password=Pass@word;");
            RegisterSqlServerDbContext(services, configuration.GetSection("ConnectionStrings")["dbConnectionString"]);
            RegisterDatabaseServices(services);
        }

        private static void RegisterCustomerService(IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
        }

        private static void RegisterDatabaseServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerDbStorage, CustomerSqlServerStorage>();
        }

        private static void RegisterSqlServerDbContext(IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(dbConnectionString));
        }

        public static void InitializeDatabaseSchema(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
            SqlServerSchemaConfiguration.InitializeDatabaseSchema(context);
        }
    }
}