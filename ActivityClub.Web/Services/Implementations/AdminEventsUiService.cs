using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Admin;

namespace ActivityClub.Web.Services.Implementations;

public sealed class AdminEventsUiService : IAdminEventsUiService
{
    private readonly IEventApiClient _api;

    public AdminEventsUiService(IEventApiClient api)
    {
        _api = api;
    }

    public async Task<List<AdminEventListVm>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _api.GetAllForAdminAsync(ct);

        return dtos.Select(e => new AdminEventListVm
        {
            EventId = e.EventId,
            Name = e.Name,
            Destination = e.Destination,
            DateFrom = e.DateFrom,
            DateTo = e.DateTo,
            Cost = e.Cost,
            CategoryName = e.CategoryName,
            StatusName = e.StatusName,
            CreatedAt = e.CreatedAt,
            IsActive = e.IsActive
        }).ToList();
    }

    public async Task<AdminEventEditVm?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await _api.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new AdminEventEditVm
        {
            Name = e.Name,
            Description = e.Description,
            CategoryId = e.CategoryId,
            Destination = e.Destination,
            DateFrom = e.DateFrom,
            DateTo = e.DateTo,
            Cost = e.Cost,
            StatusId = e.StatusId
        };
    }

    public async Task CreateAsync(AdminEventEditVm vm, CancellationToken ct = default)
    {
        var dto = new CreateEventDto
        {
            Name = vm.Name,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            Destination = vm.Destination,
            DateFrom = vm.DateFrom,
            DateTo = vm.DateTo,
            Cost = vm.Cost,
            StatusId = vm.StatusId
        };

        await _api.CreateAsync(dto, ct);
    }

    public async Task<bool> UpdateAsync(int id, AdminEventEditVm vm, CancellationToken ct = default)
    {
        var dto = new UpdateEventDto
        {
            Name = vm.Name,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            Destination = vm.Destination,
            DateFrom = vm.DateFrom,
            DateTo = vm.DateTo,
            Cost = vm.Cost,
            StatusId = vm.StatusId
        };

        return await _api.UpdateAsync(id, dto, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync(id, ct);

    public Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
    => _api.ReactivateAsync(id, ct);
}