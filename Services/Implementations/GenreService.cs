using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;

namespace Tp3.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private const int PageSize = 10;

    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<PaginatedListViewModel<GenreViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "GenreName";

        var (items, totalCount) = await _genreRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        var genreViewModels = items.Select(g => new GenreViewModel
        {
            Id = g.Id,
            GenreName = g.GenreName
        }).ToList();

        return new PaginatedListViewModel<GenreViewModel>
        {
            Items = genreViewModels,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
            SortBy = sortBy,
            Ascending = ascending
        };
    }

    public async Task<GenreViewModel?> GetDetailsAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            return null;

        return new GenreViewModel
        {
            Id = genre.Id,
            GenreName = genre.GenreName
        };
    }

    public async Task<GenreViewModel?> GetEditAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            return null;

        return new GenreViewModel
        {
            Id = genre.Id,
            GenreName = genre.GenreName
        };
    }

    public async Task CreateAsync(GenreViewModel viewModel)
    {
        var genre = new Genre
        {
            GenreName = viewModel.GenreName
        };

        await _genreRepository.AddAsync(genre);
    }

    public async Task UpdateAsync(int id, GenreViewModel viewModel)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre != null)
        {
            genre.GenreName = viewModel.GenreName;
            await _genreRepository.UpdateAsync(genre);
        }
    }

    public async Task<GenreViewModel?> GetDeleteAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            return null;

        return new GenreViewModel
        {
            Id = genre.Id,
            GenreName = genre.GenreName
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _genreRepository.DeleteAsync(id);
    }

    // LINQ Tasks Implementation

    public async Task<List<Genre>> GetTopPopularGenresAsync()
    {
        // 6. Return the 3 most popular genres (those with the most movies)
        var genres = await _genreRepository.GetAllWithMoviesAsync();
        return genres
            .OrderByDescending(g => g.Movies.Count)
            .Take(3)
            .ToList();
    }
}
