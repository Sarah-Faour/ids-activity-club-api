using ActivityClub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventsUiService _eventsUiService;

        public EventsController(IEventsUiService eventsUiService)
        {
            _eventsUiService = eventsUiService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var vms = await _eventsUiService.GetAllAsync(ct);
            return View(vms);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var vm = await _eventsUiService.GetByIdAsync(id, ct);
            if (vm == null) return NotFound();

            return View(vm);
        }
    }
}
