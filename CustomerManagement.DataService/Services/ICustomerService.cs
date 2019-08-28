using System;
using System.Threading.Tasks;
using CustomerManagement.Base.Models;

namespace CustomerManagement.DataService.Services
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> GetById(Guid id);
    }
}
