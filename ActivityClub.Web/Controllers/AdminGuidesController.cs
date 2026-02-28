using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/guides")]
public sealed class AdminGuidesController : Controller
{
    private readonly IAdminGuidesUiService _service;
    private readonly ILookupUiService _lookups;

    public AdminGuidesController(IAdminGuidesUiService service, ILookupUiService lookups)
    {
        _service = service;
        _lookups = lookups;
    }

    // GET: /admin/guides
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var guides = await _service.GetAllAsync(ct);
        return View(guides);
    }

    // GET: /admin/guides/create
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new AdminGuideEditVm
        {
            ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct)
        };

        return View(vm);
    }

    // POST: /admin/guides/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminGuideEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            vm.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);
            return View(vm);
        }

        await _service.CreateAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/guides/edit/5
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var vm = await _service.GetByIdAsync(id, ct);
        if (vm is null)
            return NotFound();

        vm.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);

        return View(vm);
    }

    // POST: /admin/guides/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminGuideEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            vm.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);
            return View(vm);
        }

        var updated = await _service.UpdateAsync(id, vm, ct);
        if (!updated)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // POST: /admin/guides/delete/5
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