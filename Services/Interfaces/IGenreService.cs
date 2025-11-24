using Tp3.Models;
using Tp3.ViewModels;

namespace Tp3.Services.Interfaces;

public interface IGenreService
{
    Task<PaginatedListViewModel<GenreViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending);
    Task<GenreViewModel?> GetDetailsAsync(int id);
    Task<GenreViewModel?> GetEditAsync(int id);
    Task CreateAsync(GenreViewModel viewModel);
    Task UpdateAsync(int id, GenreViewModel viewModel);
    Task<GenreViewModel?> GetDeleteAsync(int id);
    Task DeleteAsync(int id);
}
