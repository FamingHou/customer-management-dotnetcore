using CustomerManagement.Storage.SqlServerAdapter.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerManagement.Storage.SqlServerAdapter
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CustomerDbContext>();
            builder.UseSqlServer("Server=192.168.1.6;Database=slave;User=SA;Password=Pass@word;");
            return new CustomerDbContext(builder.Options);
        }
    }
}