using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class MoviesController : Controller
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
    {
        var vm = await _service.GetPagedAsync(page, 10, sortBy ?? "Name", ascending);
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vm = await _service.GetDetailsAsync(id);
        return View(vm);
    }

    // GET: Movies/Create
    // Displays the form to create a new movie
    public async Task<IActionResult> Create()
    {
        // Prepare the dropdown list for Genres
        ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
        return View();
    }

    // POST: Movies/Create
    // Handles the form submission
    // 'imageFile' comes from the <input type="file" name="imageFile"> in the HTML form
    [HttpPost]
    [ValidateAntiForgeryToken] // Security measure to prevent CSRF attacks
    public async Task<IActionResult> Create(MovieViewModel vm, IFormFile? imageFile)
    {
        // 1. Validate the data (checks DataAnnotations in MovieViewModel)
        if (!ModelState.IsValid)
        {
            // If invalid, reload the form with errors and the genre list
            ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
            return View(vm);
        }

        // 2. Delegate the business logic (saving to DB, handling file upload) to the Service
        await _service.CreateAsync(vm, imageFile);

        // 3. Redirect to the list page upon success
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _service.GetEditAsync(id);
        ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MovieViewModel vm, IFormFile? imageFile)
    {
        if (id != vm.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
            return View(vm);
        }

        await _service.UpdateAsync(vm, imageFile);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _service.GetDeleteAsync(id);
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
}