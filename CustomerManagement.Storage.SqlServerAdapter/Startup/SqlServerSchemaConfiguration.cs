using CustomerManagement.Storage.SqlServerAdapter.Context;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Storage.SqlServerAdapter.Startup
{
    public static class SqlServerSchemaConfiguration
    {
        public static void InitializeDatabaseSchema(CustomerDbContext context)
        {
            context.Database.Migrate();
        }
    }
}
