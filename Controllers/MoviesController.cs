using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Create()
    {
        ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieViewModel vm, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genres = new SelectList(await _service.GetAllGenresAsync(), "Id", "GenreName");
            return View(vm);
        }

        await _service.CreateAsync(vm, imageFile);
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