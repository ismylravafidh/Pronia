
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
            HomeVm homeVm = new HomeVm()
            {
                Sliders = _dbContext.Sliders.ToList(),
                Products = _dbContext.Products.Where(p => p.IsPrime == false).Include(p => p.ProductImages).ToList()
            };
            return View(homeVm);
        }
    }
}
