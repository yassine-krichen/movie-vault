using Microsoft.AspNetCore.Mvc;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipService _service;

        public MembershipsController(IMembershipService service)
        {
            _service = service;
        }

        // GET: Memberships
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            var vm = await _service.GetPagedAsync(page, 10, sortBy ?? "DurationInMonths", ascending);
            return View(vm);
        }

        // GET: Memberships/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.GetDetailsAsync(id);
            return View(vm);
        }

        // GET: Memberships/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Memberships/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Memberships/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditAsync(id);
            return View(vm);
        }

        // POST: Memberships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MembershipViewModel viewModel)
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

        // GET: Memberships/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.GetDeleteAsync(id);
            return View(vm);
        }

        // POST: Memberships/Delete/5
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
