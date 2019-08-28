using System;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.Base.Models;
using CustomerManagement.Base.Services;
using CustomerManagement.Storage.SqlServerAdapter.Context;
using CustomerManagement.Storage.SqlServerAdapter.Entity;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// @todo using Mapper
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static Customer ConvertToModel(CustomerEntity entity)
        {
            return new Customer
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
            };
        }

        private static CustomerEntity ConvertToEntity(Customer customer)
        {
            return new CustomerEntity
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
            };
        }
    }
}