using Microsoft.AspNetCore.Authorization;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TagController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
