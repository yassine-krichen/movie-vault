using Tp3.Models;
using Tp3.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tp3.Services.Interfaces;

public interface ICustomerService
{
    Task<PaginatedListViewModel<CustomerViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending);
    Task<CustomerViewModel?> GetDetailsAsync(int id);
    Task<CustomerViewModel?> GetEditAsync(int id);
    Task CreateAsync(CustomerViewModel viewModel);
    Task UpdateAsync(int id, CustomerViewModel viewModel);
    Task<CustomerViewModel?> GetDeleteAsync(int id);
    Task DeleteAsync(int id);
    Task<List<SelectListItem>> GetMembershipsDropdownAsync();
}
