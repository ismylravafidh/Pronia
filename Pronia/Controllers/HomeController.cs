using System.Collections.Generic;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm();
            homeVm.Sliders = _dbContext.Sliders.ToList();
            homeVm.Products = _dbContext.Products.Include(p=>p.Category).Include(p=>p.ProductTags).ThenInclude(p=>p.Tag).Include(p=>p.ProductImages).ToList();
            return View(homeVm);
        }
    }
}
