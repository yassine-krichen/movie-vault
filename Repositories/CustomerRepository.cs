using Microsoft.EntityFrameworkCore;
using Tp3.Data;
using Tp3.Models;

namespace Tp3.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetAllWithMembershipAsync()
        {
            return await _dbSet
                .Include(c => c.Membership)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Membership)
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<Customer?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
