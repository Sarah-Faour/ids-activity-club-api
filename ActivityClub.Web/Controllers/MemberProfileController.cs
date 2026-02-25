using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.MemberProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    [Authorize]
    public class MemberProfileController : Controller
    {
        private readonly IMemberProfileUiService _ui;
        private readonly ILookupUiService _lookups;

        public MemberProfileController(IMemberProfileUiService ui, ILookupUiService lookups)
        {
            _ui = ui;
            _lookups = lookups;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var vm = await _ui.GetProfileAsync(ct);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(CancellationToken ct)
        {
            var vm = await _ui.GetEditVmAsync(ct);

            ViewBag.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);
            ViewBag.NationalityOptions = await _lookups.GetNationalityOptionsAsync(ct);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMemberProfileVm vm, CancellationToken ct)
        {
            // ✅ Model Binding happens here
            if (!ModelState.IsValid)
            {
                // Re-load dropdowns when validation fails (must, otherwise dropdown breaks)
                ViewBag.ProfessionOptions = await _lookups.GetProfessionOptionsAsync(ct);
                ViewBag.NationalityOptions = await _lookups.GetNationalityOptionsAsync(ct);
                return View(vm);
            }

            await _ui.UpdateAsync(vm, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}