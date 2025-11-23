using Microsoft.AspNetCore.Mvc;
using Tp3.Models;
using Tp3.Repositories;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipRepository _membershipRepository;
        private const int PageSize = 10;

        public MembershipsController(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        // GET: Memberships
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, bool ascending = true)
        {
            if (page < 1) page = 1;
            sortBy ??= "DurationInMonths";

            var (items, totalCount) = await _membershipRepository.GetPagedAsync(page, PageSize, sortBy, ascending);
            
            var membershipViewModels = items.Select(m => new MembershipViewModel
            {
                Id = m.Id,
                SignupFee = m.SignupFee,
                DurationInMonths = m.DurationInMonths,
                DiscountRate = m.DiscountRate
            }).ToList();

            var viewModel = new PaginatedListViewModel<MembershipViewModel>
            {
                Items = membershipViewModels,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                PageSize = PageSize,
                TotalCount = totalCount,
                SortBy = sortBy,
                Ascending = ascending
            };

            return View(viewModel);
        }

        // GET: Memberships/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            var viewModel = new MembershipViewModel
            {
                Id = membership.Id,
                SignupFee = membership.SignupFee,
                DurationInMonths = membership.DurationInMonths,
                DiscountRate = membership.DiscountRate
            };

            return View(viewModel);
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
                var membership = new Membership
                {
                    SignupFee = viewModel.SignupFee,
                    DurationInMonths = viewModel.DurationInMonths,
                    DiscountRate = viewModel.DiscountRate
                };

                await _membershipRepository.AddAsync(membership);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Memberships/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            var viewModel = new MembershipViewModel
            {
                Id = membership.Id,
                SignupFee = membership.SignupFee,
                DurationInMonths = membership.DurationInMonths,
                DiscountRate = membership.DiscountRate
            };

            return View(viewModel);
        }

        // POST: Memberships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MembershipViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var membership = await _membershipRepository.GetByIdAsync(id);
                if (membership == null)
                {
                    return NotFound();
                }

                membership.SignupFee = viewModel.SignupFee;
                membership.DurationInMonths = viewModel.DurationInMonths;
                membership.DiscountRate = viewModel.DiscountRate;

                await _membershipRepository.UpdateAsync(membership);
                return RedirectToAction(nameof(Index));
            }

            SetViewBagErrors();
            return View(viewModel);
        }

        // GET: Memberships/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            var viewModel = new MembershipViewModel
            {
                Id = membership.Id,
                SignupFee = membership.SignupFee,
                DurationInMonths = membership.DurationInMonths,
                DiscountRate = membership.DiscountRate
            };

            return View(viewModel);
        }

        // POST: Memberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _membershipRepository.DeleteAsync(id);
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
