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

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            // @todo Mapper
            var customerEntity = new CustomerEntity
            {
                Id = Guid.NewGuid(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
            };
            var savedEntity = await _context.Customers.AddAsync(customerEntity);
            await _context.SaveChangesAsync();

            // @todo Mapper
            var returnedCustomer = new Customer
            {
                Id = savedEntity.Entity.Id,
                FirstName = savedEntity.Entity.FirstName,
                LastName = savedEntity.Entity.LastName,
            };
            return returnedCustomer;
        }
    }
}