using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;

namespace Tp3.Services.Implementations;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private const int PageSize = 10;

    public MembershipService(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task<PaginatedListViewModel<MembershipViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "DurationInMonths";

        var (items, totalCount) = await _membershipRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        var membershipViewModels = items.Select(m => new MembershipViewModel
        {
            Id = m.Id,
            SignupFee = m.SignupFee,
            DurationInMonths = m.DurationInMonths,
            DiscountRate = m.DiscountRate
        }).ToList();

        return new PaginatedListViewModel<MembershipViewModel>
        {
            Items = membershipViewModels,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
            SortBy = sortBy,
            Ascending = ascending
        };
    }

    public async Task<MembershipViewModel?> GetDetailsAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            return null;

        return new MembershipViewModel
        {
            Id = membership.Id,
            SignupFee = membership.SignupFee,
            DurationInMonths = membership.DurationInMonths,
            DiscountRate = membership.DiscountRate
        };
    }

    public async Task<MembershipViewModel?> GetEditAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            return null;

        return new MembershipViewModel
        {
            Id = membership.Id,
            SignupFee = membership.SignupFee,
            DurationInMonths = membership.DurationInMonths,
            DiscountRate = membership.DiscountRate
        };
    }

    public async Task CreateAsync(MembershipViewModel viewModel)
    {
        var membership = new Membership
        {
            SignupFee = viewModel.SignupFee,
            DurationInMonths = viewModel.DurationInMonths,
            DiscountRate = viewModel.DiscountRate
        };

        await _membershipRepository.AddAsync(membership);
    }

    public async Task UpdateAsync(int id, MembershipViewModel viewModel)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership != null)
        {
            membership.SignupFee = viewModel.SignupFee;
            membership.DurationInMonths = viewModel.DurationInMonths;
            membership.DiscountRate = viewModel.DiscountRate;
            await _membershipRepository.UpdateAsync(membership);
        }
    }

    public async Task<MembershipViewModel?> GetDeleteAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            return null;

        return new MembershipViewModel
        {
            Id = membership.Id,
            SignupFee = membership.SignupFee,
            DurationInMonths = membership.DurationInMonths,
            DiscountRate = membership.DiscountRate
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _membershipRepository.DeleteAsync(id);
    }
}
