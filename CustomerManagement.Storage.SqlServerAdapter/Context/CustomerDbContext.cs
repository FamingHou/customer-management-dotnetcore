using CustomerManagement.Storage.SqlServerAdapter.Entity;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Storage.SqlServerAdapter.Context
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}