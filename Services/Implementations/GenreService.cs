using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;
using Tp3.Exceptions;
using AutoMapper;

namespace Tp3.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public GenreService(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedListViewModel<GenreViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "GenreName";

        var (items, totalCount) = await _genreRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        var genreViewModels = _mapper.Map<List<GenreViewModel>>(items);

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

    public async Task<GenreViewModel> GetDetailsAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException("Genre", id);

        return _mapper.Map<GenreViewModel>(genre);
    }

    public async Task<GenreViewModel> GetEditAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException("Genre", id);

        return _mapper.Map<GenreViewModel>(genre);
    }

    public async Task CreateAsync(GenreViewModel viewModel)
    {
        var genre = _mapper.Map<Genre>(viewModel);
        await _genreRepository.AddAsync(genre);
    }

    public async Task UpdateAsync(int id, GenreViewModel viewModel)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException("Genre", id);

        _mapper.Map(viewModel, genre);
        await _genreRepository.UpdateAsync(genre);
    }

    public async Task<GenreViewModel> GetDeleteAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException("Genre", id);

        return _mapper.Map<GenreViewModel>(genre);
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
