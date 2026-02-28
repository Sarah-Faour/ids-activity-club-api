using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/events")]
public sealed class AdminEventsController : Controller
{
    private readonly IAdminEventsUiService _service;
    private readonly ILookupUiService _lookups;

    public AdminEventsController(IAdminEventsUiService service, ILookupUiService lookups)
    {
        _service = service;
        _lookups = lookups;
    }

    // GET: /admin/events
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var events = await _service.GetAllAsync(ct);
        return View(events);
    }

    // GET: /admin/events/create
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new AdminEventEditVm
        {
            CategoryOptions = await _lookups.GetEventCategoryOptionsAsync(ct),
            StatusOptions = await _lookups.GetEventStatusOptionsAsync(ct)
        };

        return View(vm);
    }

    // POST: /admin/events/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminEventEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            // repopulate dropdowns
            vm.CategoryOptions = await _lookups.GetEventCategoryOptionsAsync(ct);
            vm.StatusOptions = await _lookups.GetEventStatusOptionsAsync(ct);
            return View(vm);
        }

        await _service.CreateAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/events/edit/5
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var vm = await _service.GetByIdAsync(id, ct);
        if (vm is null)
            return NotFound();

        vm.CategoryOptions = await _lookups.GetEventCategoryOptionsAsync(ct);
        vm.StatusOptions = await _lookups.GetEventStatusOptionsAsync(ct);

        return View(vm);
    }

    // POST: /admin/events/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminEventEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            vm.CategoryOptions = await _lookups.GetEventCategoryOptionsAsync(ct);
            vm.StatusOptions = await _lookups.GetEventStatusOptionsAsync(ct);
            return View(vm);
        }
        var updated = await _service.UpdateAsync(id, vm, ct);
        if (!updated)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // POST: /admin/events/delete/5
    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("reactivate/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reactivate(int id, CancellationToken ct)
    {
        await _service.ReactivateAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}