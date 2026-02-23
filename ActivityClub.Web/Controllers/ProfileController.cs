using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileUiService _ui;

        public ProfileController(IProfileUiService ui)
        {
            _ui = ui;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var vm = await _ui.GetProfileAsync(ct);
            return View(vm);
        }

        [HttpGet]
        public IActionResult EditEmail()
        {
            return View(new UpdateEmailVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(UpdateEmailVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            await _ui.UpdateEmailAsync(vm, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            await _ui.ChangePasswordAsync(vm, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}