using Microsoft.EntityFrameworkCore;
using Tp3.Data;
using Tp3.Models;

namespace Tp3.Repositories
{
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Membership>> GetAllWithCustomersAsync()
        {
            return await _dbSet
                .Include(m => m.Customers)
                .ToListAsync();
        }
    }
}
