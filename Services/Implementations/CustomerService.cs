using Tp3.Models;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;
using Tp3.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tp3.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMembershipRepository _membershipRepository;
    private const int PageSize = 10;

    public CustomerService(ICustomerRepository customerRepository, IMembershipRepository membershipRepository)
    {
        _customerRepository = customerRepository;
        _membershipRepository = membershipRepository;
    }

    public async Task<PaginatedListViewModel<CustomerViewModel>> GetPagedAsync(int page, int pageSize, string sortBy, bool ascending)
    {
        if (page < 1) page = 1;
        sortBy ??= "Name";

        var (items, totalCount) = await _customerRepository.GetPagedAsync(page, pageSize, sortBy, ascending);

        var customers = await _customerRepository.GetAllWithMembershipAsync();
        var pagedCustomers = customers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        return new PaginatedListViewModel<CustomerViewModel>
        {
            Items = pagedCustomers,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
            SortBy = sortBy,
            Ascending = ascending
        };
    }

    public async Task<CustomerViewModel?> GetDetailsAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
        if (customer == null)
            return null;

        return new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            MembershipId = customer.MembershipId,
            MembershipInfo = customer.Membership != null
                ? $"Fee: ${customer.Membership.SignupFee}, Duration: {customer.Membership.DurationInMonths} months, Discount: {customer.Membership.DiscountRate}%"
                : "No membership"
        };
    }

    public async Task<CustomerViewModel?> GetEditAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return null;

        return new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            MembershipId = customer.MembershipId
        };
    }

    public async Task CreateAsync(CustomerViewModel viewModel)
    {
        var customer = new Customer
        {
            Name = viewModel.Name,
            MembershipId = viewModel.MembershipId
        };

        await _customerRepository.AddAsync(customer);
    }

    public async Task UpdateAsync(int id, CustomerViewModel viewModel)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer != null)
        {
            customer.Name = viewModel.Name;
            customer.MembershipId = viewModel.MembershipId;
            await _customerRepository.UpdateAsync(customer);
        }
    }

    public async Task<CustomerViewModel?> GetDeleteAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(id);
        if (customer == null)
            return null;

        return new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            MembershipId = customer.MembershipId,
            MembershipInfo = customer.Membership != null
                ? $"Fee: ${customer.Membership.SignupFee}, Duration: {customer.Membership.DurationInMonths} months"
                : "No membership"
        };
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
