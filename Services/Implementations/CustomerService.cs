using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tp3.Exceptions;
using AutoMapper;

namespace Tp3.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public CustomerService(ICustomerRepository customerRepository, IMembershipRepository membershipRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _membershipRepository = membershipRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedListViewModel<CustomerViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "Name";

        var (items, totalCount) = await _customerRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        // Note: Ideally we should fix the repository to support Includes in GetPagedAsync
        // For now, we keep the existing logic of fetching all to ensure Membership is included
        var customers = await _customerRepository.GetAllWithMembershipAsync();
        
        var pagedItems = customers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var customerViewModels = _mapper.Map<List<CustomerViewModel>>(pagedItems);

        return new PaginatedListViewModel<CustomerViewModel>
        {
            Items = customerViewModels,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
            SortBy = sortBy,
            Ascending = ascending
        };
    }

    public async Task<CustomerViewModel> GetDetailsAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
        if (customer == null)
            throw new NotFoundException("Customer", id);

        return _mapper.Map<CustomerViewModel>(customer);
    }

    public async Task<CustomerViewModel> GetEditAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new NotFoundException("Customer", id);

        return _mapper.Map<CustomerViewModel>(customer);
    }

    public async Task CreateAsync(CustomerViewModel viewModel)
    {
        var customer = _mapper.Map<Customer>(viewModel);
        await _customerRepository.AddAsync(customer);
    }

    public async Task UpdateAsync(int id, CustomerViewModel viewModel)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new NotFoundException("Customer", id);

        _mapper.Map(viewModel, customer);
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<CustomerViewModel> GetDeleteAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
        if (customer == null)
            throw new NotFoundException("Customer", id);

        return _mapper.Map<CustomerViewModel>(customer);
    }

    public async Task DeleteAsync(int id)
    {
        await _customerRepository.DeleteAsync(id);
    }

    public async Task<List<SelectListItem>> GetMembershipsDropdownAsync()
    {
        var memberships = await _membershipRepository.GetAllAsync();
        return memberships.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.DurationInMonths} months"
        }).ToList();
    }

    // LINQ Tasks Implementation

    public async Task<List<Customer>> GetSubscribedCustomersWithDiscountAsync()
    {
        // 4. Find all customers subscribed to the newsletter having a membership with a discount > 10%
        var customers = await _customerRepository.GetAllWithMembershipAsync();
        return customers
            .Where(c => c.IsSubscribedToNewsletter && c.Membership?.DiscountRate > 10)
            .ToList();
    }
}
