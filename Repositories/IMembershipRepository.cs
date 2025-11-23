using Tp3.Models;

namespace Tp3.Repositories
{
    public interface IMembershipRepository : IRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetAllWithCustomersAsync();
    }
}
