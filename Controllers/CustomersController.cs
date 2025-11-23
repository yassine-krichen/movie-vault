using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tp3.Models;
using Tp3.Repositories;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMembershipRepository _membershipRepository;
        private const int PageSize = 10;

        public CustomersController(ICustomerRepository customerRepository, IMembershipRepository membershipRepository)
        {
            _customerRepository = customerRepository;
            _membershipRepository = membershipRepository;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            if (page < 1) page = 1;
            sortBy ??= "Name";

            var (items, totalCount) = await _customerRepository.GetPagedAsync(page, PageSize, sortBy, ascending);
            
            var customers = await _customerRepository.GetAllWithMembershipAsync();
            var pagedCustomers = customers
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    MembershipId = c.MembershipId,
                    MembershipInfo = c.Membership != null 
                        ? $"{c.Membership.DurationInMonths} months - {c.Membership.DiscountRate}% discount"
                        : "No membership"
                })
                .ToList();

            var viewModel = new PaginatedListViewModel<CustomerViewModel>
            {
                Items = pagedCustomers,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                PageSize = PageSize,
                TotalCount = totalCount,
                SortBy = sortBy,
                Ascending = ascending
            };

            return View(viewModel);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                MembershipId = customer.MembershipId,
                MembershipInfo = customer.Membership != null 
                    ? $"Fee: ${customer.Membership.SignupFee}, Duration: {customer.Membership.DurationInMonths} months, Discount: {customer.Membership.DiscountRate}%"
                    : "No membership"
            };

            return View(viewModel);
        }

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            await PopulateMembershipsDropdown();
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    Name = viewModel.Name,
                    MembershipId = viewModel.MembershipId
                };

                await _customerRepository.AddAsync(customer);
                return RedirectToAction(nameof(Index));
            }

            await PopulateMembershipsDropdown();
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                MembershipId = customer.MembershipId
            };

            await PopulateMembershipsDropdown();
            return View(viewModel);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.Name = viewModel.Name;
                customer.MembershipId = viewModel.MembershipId;

                await _customerRepository.UpdateAsync(customer);
                return RedirectToAction(nameof(Index));
            }

            await PopulateMembershipsDropdown();
            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                MembershipId = customer.MembershipId,
                MembershipInfo = customer.Membership != null 
                    ? $"Fee: ${customer.Membership.SignupFee}, Duration: {customer.Membership.DurationInMonths} months"
                    : "No membership"
            };

            return View(viewModel);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateMembershipsDropdown()
        {
            var memberships = await _membershipRepository.GetAllAsync();
            ViewBag.Memberships = new SelectList(memberships, "Id", "DurationInMonths");
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
