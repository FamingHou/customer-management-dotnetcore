using System.Threading.Tasks;
using CustomerManagement.Base.Models;

namespace CustomerManagement.Base.Services
{
    public interface ICustomerDbStorage
    {
        Task<int> CreateCustomer(Customer customer);
    }
}
