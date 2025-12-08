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

    /// <summary>
    /// Creates a new movie and handles the optional image upload.
    /// </summary>
    /// <param name="viewModel">The data from the form.</param>
    /// <param name="imageFile">The uploaded file (if any).</param>
    public async Task<Movie> CreateAsync(MovieViewModel viewModel, IFormFile? imageFile)
    {
        // 1. Map the ViewModel (View data) to the Entity (Database data)
        var movie = new Movie
        {
            Name = viewModel.Name,
            GenreId = viewModel.GenreId,
            DateAjoutMovie = viewModel.DateAjoutMovie ?? DateTime.Now
        };

        // 2. Handle the image upload logic
        // This saves the file to disk and returns the relative path (e.g., "/images/movies/guid.jpg")
        movie.ImageFile = await UploadImageAsync(imageFile);

        // 3. Save the new entity to the database via the Repository
        await _movies.AddAsync(movie);

        return movie;
    }

    public async Task<Movie> UpdateAsync(MovieViewModel viewModel, IFormFile? imageFile)
    {
        // 1. Fetch the existing movie from DB
        var movie = await _movies.GetByIdAsync(viewModel.Id);
        if (movie == null) 
            throw new NotFoundException("Movie", viewModel.Id);

        // 2. Update properties
        movie.Name = viewModel.Name;
        movie.GenreId = viewModel.GenreId;
        movie.DateAjoutMovie = viewModel.DateAjoutMovie;

        // 3. Handle Image Update
        if (imageFile != null)
        {
            // If a new image is uploaded, delete the old one to save space
            DeleteImageIfExists(movie.ImageFile);
            // Upload the new one
            movie.ImageFile = await UploadImageAsync(imageFile);
        }

        // 4. Save changes
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

    /// <summary>
    /// Helper method to save an uploaded file to the server's disk.
    /// </summary>
    /// <param name="file">The file uploaded by the user.</param>
    /// <returns>The relative path to the saved file (for the DB), or null if no file.</returns>
    private async Task<string?> UploadImageAsync(IFormFile? file)
    {
        // Check if a file was actually uploaded
        if (file == null || file.Length == 0) return null;

        // 1. Define the storage path: wwwroot/images/movies
        // _env.WebRootPath points to the 'wwwroot' folder
        var folder = Path.Combine(_env.WebRootPath, "images", "movies");
        Directory.CreateDirectory(folder); // Ensure folder exists

        // 2. Generate a unique filename to prevent overwriting existing files
        // Guid.NewGuid() creates a random string like "e02fd0e4-..."
        var fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
        
        // 3. Combine folder and filename to get the full physical path
        var path = Path.Combine(folder, fileName);

        // 4. Save the file stream to the disk
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        // 5. Return the relative path (URL) to be stored in the database
        // This is what we use in <img src="...">
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
