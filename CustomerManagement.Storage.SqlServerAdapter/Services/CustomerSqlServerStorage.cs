using System;
using System.Threading.Tasks;
using CustomerManagement.Base.Models;
using CustomerManagement.Base.Services;
using CustomerManagement.Storage.SqlServerAdapter.Context;
using CustomerManagement.Storage.SqlServerAdapter.Entity;
using NLog;

namespace CustomerManagement.Storage.SqlServerAdapter.Services
{
    public class CustomerSqlServerStorage : ICustomerDbStorage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CustomerDbContext _context;

        public CustomerSqlServerStorage(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCustomer(Customer customer)
        {
            var customerEntity = new CustomerEntity
            {
                Id = Guid.NewGuid(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
            };
            _context.Customers.Add(customerEntity);
            return await _context.SaveChangesAsync();
        }
    }
}