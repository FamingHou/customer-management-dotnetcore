using System;
using System.Threading.Tasks;
using CustomerManagement.Base.Models;
using CustomerManagement.Base.Services;

namespace CustomerManagement.DataService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDbStorage _customerDbStorage;

        public CustomerService(ICustomerDbStorage customerDbStorage)
        {
            _customerDbStorage = customerDbStorage;
        }

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
