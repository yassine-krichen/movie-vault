using Microsoft.EntityFrameworkCore;
using Tp3.Data;
using Tp3.Models;
using Tp3.Repositories.Interfaces;

namespace Tp3.Repositories.Implementations
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Genre>> GetAllWithMoviesAsync()
        {
            return await _dbSet
                .Include(g => g.Movies)
                .ToListAsync();
        }
    }
}
