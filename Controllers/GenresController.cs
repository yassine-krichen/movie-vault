using Microsoft.AspNetCore.Mvc;
using Tp3.Models;
using Tp3.Repositories;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class GenresController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private const int PageSize = 10;

        public GenresController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        // GET: Genres
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            if (page < 1) page = 1;
            sortBy ??= "GenreName";

            var (items, totalCount) = await _genreRepository.GetPagedAsync(page, PageSize, sortBy, ascending);
            
            var genreViewModels = items.Select(g => new GenreViewModel
            {
                Id = g.Id,
                GenreName = g.GenreName
            }).ToList();

            var viewModel = new PaginatedListViewModel<GenreViewModel>
            {
                Items = genreViewModels,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                PageSize = PageSize,
                TotalCount = totalCount,
                SortBy = sortBy,
                Ascending = ascending
            };

            return View(viewModel);
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            var viewModel = new GenreViewModel
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };

            return View(viewModel);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre
                {
                    GenreName = viewModel.GenreName
                };

                await _genreRepository.AddAsync(genre);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            var viewModel = new GenreViewModel
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };

            return View(viewModel);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var genre = await _genreRepository.GetByIdAsync(id);
                if (genre == null)
                {
                    return NotFound();
                }

                genre.GenreName = viewModel.GenreName;

                await _genreRepository.UpdateAsync(genre);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            var viewModel = new GenreViewModel
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };

            return View(viewModel);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _genreRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
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
