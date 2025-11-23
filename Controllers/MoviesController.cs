using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tp3.Models;
using Tp3.Repositories;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 10;

        public MoviesController(IMovieRepository movieRepository, IGenreRepository genreRepository, IWebHostEnvironment webHostEnvironment)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            if (page < 1) page = 1;
            sortBy ??= "Name";

            var (items, totalCount) = await _movieRepository.GetPagedAsync(page, PageSize, sortBy, ascending);
            
            var movies = await _movieRepository.GetAllWithGenreAsync();
            var pagedMovies = movies
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    GenreId = m.GenreId,
                    GenreName = m.Genre?.GenreName ?? "Unknown",
                    ImageFile = m.ImageFile,
                    DateAjoutMovie = m.DateAjoutMovie
                })
                .ToList();

            var viewModel = new PaginatedListViewModel<MovieViewModel>
            {
                Items = pagedMovies,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                PageSize = PageSize,
                TotalCount = totalCount,
                SortBy = sortBy,
                Ascending = ascending
            };

            return View(viewModel);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                GenreId = movie.GenreId,
                GenreName = movie.Genre?.GenreName ?? "Unknown",
                ImageFile = movie.ImageFile,
                DateAjoutMovie = movie.DateAjoutMovie
            };

            return View(viewModel);
        }

        // GET: Movies/Create
        public async Task<IActionResult> Create()
        {
            await PopulateGenresDropdown();
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel viewModel, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Name = viewModel.Name,
                    GenreId = viewModel.GenreId,
                    DateAjoutMovie = viewModel.DateAjoutMovie ?? DateTime.Now
                };

                // Handle image file upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "movies");
                    Directory.CreateDirectory(uploadsFolder); // Ensure directory exists
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    movie.ImageFile = "/images/movies/" + uniqueFileName;
                }

                await _movieRepository.AddAsync(movie);
                return RedirectToAction(nameof(Index));
            }

            await PopulateGenresDropdown();
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                GenreId = movie.GenreId,
                ImageFile = movie.ImageFile,
                DateAjoutMovie = movie.DateAjoutMovie
            };

            await PopulateGenresDropdown();
            return View(viewModel);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieViewModel viewModel, IFormFile? imageFile)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var movie = await _movieRepository.GetByIdAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }

                movie.Name = viewModel.Name;
                movie.GenreId = viewModel.GenreId;
                movie.DateAjoutMovie = viewModel.DateAjoutMovie;

                // Handle image file upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(movie.ImageFile))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageFile.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Upload new image
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "movies");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    movie.ImageFile = "/images/movies/" + uniqueFileName;
                }

                await _movieRepository.UpdateAsync(movie);
                return RedirectToAction(nameof(Index));
            }

            await PopulateGenresDropdown();
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                GenreId = movie.GenreId,
                GenreName = movie.Genre?.GenreName ?? "Unknown",
                ImageFile = movie.ImageFile,
                DateAjoutMovie = movie.DateAjoutMovie
            };

            return View(viewModel);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie != null && !string.IsNullOrEmpty(movie.ImageFile))
            {
                // Delete image file if exists
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageFile.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _movieRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateGenresDropdown()
        {
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "GenreName");
        }

        private void SetViewBagErrors()
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }
        }
    }
}
