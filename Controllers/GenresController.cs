using Microsoft.AspNetCore.Mvc;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class GenresController : Controller
    {
        private readonly IGenreService _service;

        public GenresController(IGenreService service)
        {
            _service = service;
        }

        // GET: Genres
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            var vm = await _service.GetPagedAsync(page, 10, sortBy ?? "GenreName", ascending);
            return View(vm);
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.GetDetailsAsync(id);
            return View(vm);
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
                await _service.CreateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditAsync(id);
            return View(vm);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, viewModel);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.GetDeleteAsync(id);
            return View(vm);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
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
