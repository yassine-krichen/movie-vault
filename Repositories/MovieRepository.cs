using Microsoft.EntityFrameworkCore;
using Tp3.Data;
using Tp3.Models;

namespace Tp3.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Movie>> GetAllWithGenreAsync()
        {
            return await _dbSet
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Genre)
                .Include(m => m.Customers)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<Movie?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
