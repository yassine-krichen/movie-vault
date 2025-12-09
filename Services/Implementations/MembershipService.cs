using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;
using Tp3.Exceptions;
using AutoMapper;

namespace Tp3.Services.Implementations;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public MembershipService(IMembershipRepository membershipRepository, IMapper mapper)
    {
        _membershipRepository = membershipRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedListViewModel<MembershipViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "DurationInMonths";

        var (items, totalCount) = await _membershipRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        var membershipViewModels = _mapper.Map<List<MembershipViewModel>>(items);

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

    public async Task<MembershipViewModel> GetDetailsAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            throw new NotFoundException("Membership", id);

        return _mapper.Map<MembershipViewModel>(membership);
    }

    public async Task<MembershipViewModel> GetEditAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            throw new NotFoundException("Membership", id);

        return _mapper.Map<MembershipViewModel>(membership);
    }

    public async Task CreateAsync(MembershipViewModel viewModel)
    {
        var membership = _mapper.Map<Membership>(viewModel);
        await _membershipRepository.AddAsync(membership);
    }

    public async Task UpdateAsync(int id, MembershipViewModel viewModel)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            throw new NotFoundException("Membership", id);

        _mapper.Map(viewModel, membership);
        await _membershipRepository.UpdateAsync(membership);
    }

    public async Task<MembershipViewModel> GetDeleteAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            throw new NotFoundException("Membership", id);

        return _mapper.Map<MembershipViewModel>(membership);
    }

    public async Task DeleteAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
            throw new NotFoundException("Membership", id);

        await _membershipRepository.DeleteAsync(id);
    }
}
