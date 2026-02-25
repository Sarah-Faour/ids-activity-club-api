using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.GuideProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    [Authorize(Roles = "Guide")]
    public class GuideProfileController : Controller
    {
        private readonly IGuideProfileUiService _ui;
        private readonly ILookupUiService _lookups;

        public GuideProfileController(IGuideProfileUiService ui, ILookupUiService lookups)
        {
            _ui = ui;
            _lookups = lookups;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var vm = await _ui.GetProfileAsync(ct);
            if (vm is null) return NotFound();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(CancellationToken ct)
        {
            var vm = await _ui.GetEditVmAsync(ct);
            if (vm is null) return NotFound();

            vm.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditGuideProfileVm vm, CancellationToken ct)
        {
            vm.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);

            if (!ModelState.IsValid)
                return View(vm);

            await _ui.UpdateAsync(vm, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}