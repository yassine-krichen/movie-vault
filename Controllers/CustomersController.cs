using Microsoft.AspNetCore.Mvc;
using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            var vm = await _service.GetPagedAsync(page, 10, sortBy ?? "Name", ascending);
            return View(vm);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.GetDetailsAsync(id);
            return View(vm);
        }

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Memberships = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _service.GetMembershipsDropdownAsync(), "Value", "Text");
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Memberships = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _service.GetMembershipsDropdownAsync(), "Value", "Text");
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditAsync(id);
            ViewBag.Memberships = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _service.GetMembershipsDropdownAsync(), "Value", "Text");
            return View(vm);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, viewModel);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Memberships = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _service.GetMembershipsDropdownAsync(), "Value", "Text");
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.GetDeleteAsync(id);
            return View(vm);
        }

        // POST: Customers/Delete/5
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
