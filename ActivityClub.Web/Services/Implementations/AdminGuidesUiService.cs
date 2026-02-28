using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Admin;

namespace ActivityClub.Web.Services.Implementations;

public sealed class AdminGuidesUiService : IAdminGuidesUiService
{
    private readonly IGuideApiClient _api;

    public AdminGuidesUiService(IGuideApiClient api)
    {
        _api = api;
    }

    public async Task<List<AdminGuideListVm>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _api.GetAllForAdminAsync(ct);

        return dtos.Select(g => new AdminGuideListVm
        {
            GuideId = g.GuideId,
            UserId = g.UserId,
            FullName = g.FullName,
            JoiningDate = g.JoiningDate,
            ProfessionName = g.ProfessionName,
            Photo = g.Photo,
            IsActive = g.IsActive
        }).ToList();
    }

    public async Task<AdminGuideEditVm?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var g = await _api.GetByIdAsync(id, ct);
        if (g == null) return null;

        return new AdminGuideEditVm
        {
            UserId = g.UserId,
            FullName = g.FullName,
            JoiningDate = g.JoiningDate,
            Photo = g.Photo,
            ProfessionId = g.ProfessionId
        };
    }

    public async Task CreateAsync(AdminGuideEditVm vm, CancellationToken ct = default)
    {
        var dto = new CreateGuideDto
        {
            UserId = vm.UserId,
            FullName = vm.FullName,
            JoiningDate = vm.JoiningDate,
            Photo = vm.Photo,
            ProfessionId = vm.ProfessionId
        };

        await _api.CreateAsync(dto, ct);
    }

    public async Task<bool> UpdateAsync(int id, AdminGuideEditVm vm, CancellationToken ct = default)
    {
        var dto = new UpdateGuideDto
        {
            FullName = vm.FullName,
            JoiningDate = vm.JoiningDate,
            Photo = vm.Photo,
            ProfessionId = vm.ProfessionId
        };

        return await _api.UpdateAsync(id, dto, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync(id, ct);

    public Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
    => _api.ReactivateAsync(id, ct);
}