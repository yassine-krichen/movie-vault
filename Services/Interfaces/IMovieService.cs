using Tp3.Models;
using Tp3.ViewModels;

namespace Tp3.Services.Interfaces;

public interface IMovieService
{
    Task<PaginatedListViewModel<MovieViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending);
    Task<MovieViewModel?> GetDetailsAsync(int id);
    Task<MovieViewModel?> GetEditAsync(int id);
    Task<Movie?> CreateAsync(MovieViewModel viewModel, IFormFile? imageFile);
    Task<Movie?> UpdateAsync(MovieViewModel viewModel, IFormFile? imageFile);
    Task<MovieViewModel?> GetDeleteAsync(int id);
    Task DeleteAsync(int id);
    Task<List<Genre>> GetAllGenresAsync();
}
