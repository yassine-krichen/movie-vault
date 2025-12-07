using Tp3.Models;
using Tp3.ViewModels;

namespace Tp3.Services.Interfaces;

public interface IMembershipService
{
    Task<PaginatedListViewModel<MembershipViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending);
    Task<MembershipViewModel> GetDetailsAsync(int id);
    Task<MembershipViewModel> GetEditAsync(int id);
    Task CreateAsync(MembershipViewModel viewModel);
    Task UpdateAsync(int id, MembershipViewModel viewModel);
    Task<MembershipViewModel> GetDeleteAsync(int id);
    Task DeleteAsync(int id);
}
