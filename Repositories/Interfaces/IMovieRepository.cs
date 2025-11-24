using Tp3.Models;

namespace Tp3.Repositories.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAllWithGenreAsync();
        Task<Movie?> GetByIdWithDetailsAsync(int id);
    }
}
