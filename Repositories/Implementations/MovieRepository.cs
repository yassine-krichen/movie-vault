using Microsoft.EntityFrameworkCore;
using Tp3.Data;
using Tp3.Models;
using Tp3.Repositories.Interfaces;

namespace Tp3.Repositories.Implementations
{
    /// <summary>
    /// The Repository handles direct database access.
    /// It abstracts EF Core logic (DbSet, Include, etc.) from the rest of the app.
    /// </summary>
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Example of Eager Loading: Fetching related data (Genre) in a single query
        public async Task<IEnumerable<Movie>> GetAllWithGenreAsync()
        {
            return await _dbSet
                .Include(m => m.Genre) // SQL JOIN: Fetches Genre data along with Movie
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Genre)
                .Include(m => m.Customers) // Fetching multiple related entities
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
