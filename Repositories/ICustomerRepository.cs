using Tp3.Models;

namespace Tp3.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetAllWithMembershipAsync();
        Task<Customer?> GetByIdWithDetailsAsync(int id);
    }
}
