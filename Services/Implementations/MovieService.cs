using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tp3.Models;
using Tp3.Repositories.Interfaces;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Exceptions;

namespace Tp3.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movies;
    private readonly IGenreRepository _genres;
    private readonly IWebHostEnvironment _env;
    private const int PageSize = 10;

    public MovieService(IMovieRepository movies, IGenreRepository genres, IWebHostEnvironment env)
    {
        _movies = movies;
        _genres = genres;
        _env = env;
    }

    public async Task<PaginatedListViewModel<MovieViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        var (items, totalCount) = await _movies.GetPagedAsync(page, pageSize, sortBy, ascending);

        var movies = items.Select(m => new MovieViewModel
        {
            Id = m.Id,
            Name = m.Name,
            GenreId = m.GenreId,
            GenreName = m.Genre?.GenreName,
            ImageFile = m.ImageFile,
            DateAjoutMovie = m.DateAjoutMovie
        }).ToList();

        return new PaginatedListViewModel<MovieViewModel>
        {
            Items = movies,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
            SortBy = sortBy,
            Ascending = ascending
        };
    }

    public async Task<MovieViewModel> GetDetailsAsync(int id)
    {
        var movie = await _movies.GetByIdWithDetailsAsync(id);
        if (movie == null) 
            throw new NotFoundException("Movie", id);

        return new MovieViewModel
        {
            Id = movie.Id,
            Name = movie.Name,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.GenreName,
            ImageFile = movie.ImageFile,
            DateAjoutMovie = movie.DateAjoutMovie
        };
    }

    public async Task<MovieViewModel> GetEditAsync(int id)
    {
        var movie = await _movies.GetByIdAsync(id);
        if (movie == null) 
            throw new NotFoundException("Movie", id);

        return new MovieViewModel
        {
            Id = movie.Id,
            Name = movie.Name,
            GenreId = movie.GenreId,
            ImageFile = movie.ImageFile,
            DateAjoutMovie = movie.DateAjoutMovie
        };
    }

    public async Task<Movie> CreateAsync(MovieViewModel viewModel, IFormFile? imageFile)
    {
        var movie = new Movie
        {
            Name = viewModel.Name,
            GenreId = viewModel.GenreId,
            DateAjoutMovie = viewModel.DateAjoutMovie ?? DateTime.Now
        };

        movie.ImageFile = await UploadImageAsync(imageFile);
        await _movies.AddAsync(movie);

        return movie;
    }

    public async Task<Movie> UpdateAsync(MovieViewModel viewModel, IFormFile? imageFile)
    {
        var movie = await _movies.GetByIdAsync(viewModel.Id);
        if (movie == null) 
            throw new NotFoundException("Movie", viewModel.Id);

        movie.Name = viewModel.Name;
        movie.GenreId = viewModel.GenreId;
        movie.DateAjoutMovie = viewModel.DateAjoutMovie;

        if (imageFile != null)
        {
            DeleteImageIfExists(movie.ImageFile);
            movie.ImageFile = await UploadImageAsync(imageFile);
        }

        await _movies.UpdateAsync(movie);
        return movie;
    }

    public async Task<MovieViewModel> GetDeleteAsync(int id)
    {
        var movie = await _movies.GetByIdWithDetailsAsync(id);
        if (movie == null) 
            throw new NotFoundException("Movie", id);

        return new MovieViewModel
        {
            Id = movie.Id,
            Name = movie.Name,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.GenreName,
            ImageFile = movie.ImageFile,
            DateAjoutMovie = movie.DateAjoutMovie
        };
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await _movies.GetByIdAsync(id);
        if (movie == null)
            throw new NotFoundException("Movie", id);

        if (movie.ImageFile != null)
            DeleteImageIfExists(movie.ImageFile);

        await _movies.DeleteAsync(id);
    }

    public async Task<List<Genre>> GetAllGenresAsync()
    {
        var res = await _genres.GetAllAsync();
         return res.ToList(); // check if this is impl correctly
    }

    private async Task<string?> UploadImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var folder = Path.Combine(_env.WebRootPath, "images", "movies");
        Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return "/images/movies/" + fileName;
    }

    private void DeleteImageIfExists(string? path)
    {
        if (string.IsNullOrEmpty(path)) return;

        var physical = Path.Combine(_env.WebRootPath, path.TrimStart('/'));
        if (File.Exists(physical))
            File.Delete(physical);
    }

    // LINQ Tasks Implementation

    public async Task<List<Movie>> GetAvailableActionMoviesAsync()
    {
        // 1. List all movies associated with the genre "Action" and where stock > 0
        var movies = await _movies.GetAllWithGenreAsync();
        return movies
            .Where(m => m.Genre?.GenreName == "Action" && m.Stock > 0)
            .ToList();
    }

    public async Task<List<Movie>> GetMoviesOrderedByDateAndNameAsync()
    {
        // 2. List all movies ordered by release date then by title alphabetically
        var movies = await _movies.GetAllAsync();
        return movies
            .OrderBy(m => m.DateAjoutMovie)
            .ThenBy(m => m.Name)
            .ToList();
    }

    public async Task<int> GetTotalMovieCountAsync()
    {
        // 3. Calculate the total number of movies in the store
        var movies = await _movies.GetAllAsync();
        return movies.Count();
    }

    public async Task<List<MovieGenreViewModel>> GetMoviesWithGenresAsync()
    {
        // 5. Display the list of movies with their genre using a join between Movie and Genre
        var movies = await _movies.GetAllAsync();
        var genres = await _genres.GetAllAsync();

        var query = from m in movies
                    join g in genres on m.GenreId equals g.Id
                    select new MovieGenreViewModel
                    {
                        MovieTitle = m.Name,
                        GenreName = g.GenreName
                    };

        return query.ToList();
    }
}
