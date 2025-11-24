using Tp3.Models;

namespace Tp3.Repositories.Interfaces
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<IEnumerable<Genre>> GetAllWithMoviesAsync();
    }
}
