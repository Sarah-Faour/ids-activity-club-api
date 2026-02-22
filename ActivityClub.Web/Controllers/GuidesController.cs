using ActivityClub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    public class GuidesController : Controller
    {
        private readonly IGuidesUiService _ui;

        public GuidesController(IGuidesUiService ui)
        {
            _ui = ui;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _ui.GetListAsync(ct);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var vm = await _ui.GetDetailsAsync(id, ct);
            if (vm is null) return NotFound();

            return View(vm);
        }
    }
}