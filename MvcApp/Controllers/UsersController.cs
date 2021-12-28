using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using MvcApp.Services;

namespace MvcApp.Controllers
{
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService service)
        {
            _userService = service;
        }

        [AuthorizeForScopes(ScopeKeySection = "WebApi:Scope")]
        public async Task<ActionResult> Index()
        {
            return View(await _userService.GetAsync());
        }
    }
}