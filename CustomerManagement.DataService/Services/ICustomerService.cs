using System.Threading.Tasks;
using CustomerManagement.Base.Models;

namespace CustomerManagement.DataService.Services
{
    public interface ICustomerService
    {
        Task<int> CreateCustomer(Customer customer);
    }
}
