using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    public sealed class AuthController : Controller
    {
        private readonly IAuthUiService _authUiService;
        private readonly ILookupUiService _lookupUiService;

        public AuthController(IAuthUiService authUiService, ILookupUiService lookupUiService)
        {
            _authUiService = authUiService;
            _lookupUiService = lookupUiService;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var ok = await _authUiService.LoginAsync(vm, Response, ct);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Register(CancellationToken ct)
        {
            var vm = new RegisterVm
            {
                GenderOptions = await _lookupUiService.GetGenderOptionsAsync(ct)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm, CancellationToken ct)
        {
            vm.GenderOptions = await _lookupUiService.GetGenderOptionsAsync(ct);

            if (!ModelState.IsValid)
                return View(vm);

            await _authUiService.RegisterAsync(vm, Response, ct);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _authUiService.Logout(Response);
            return RedirectToAction("Index", "Home");
        }
    }
}