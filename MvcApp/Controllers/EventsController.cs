using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using MvcApp.Services;

namespace MvcApp.Controllers
{
    public class EventsController : Controller
    {
        private IEventService _eventService;

        public EventsController(IEventService service)
        {
            _eventService = service;
        }

        [AuthorizeForScopes(ScopeKeySection = "WebApi:Scope")]
        public async Task<ActionResult> Index()
        {
            return View(await _eventService.GetAsync());
        }
    }
}