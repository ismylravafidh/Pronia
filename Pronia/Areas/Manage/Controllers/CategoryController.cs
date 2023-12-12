using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        AppDbContext _dbContext;
        
        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            List<Category> categories = _dbContext.Categories.Include(p => p.Products).ToList();
            return View(categories);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            Category category = _dbContext.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category oldCategory = _dbContext.Categories.Find(newCategory.Id);
            oldCategory.Name = newCategory.Name;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            Category category = _dbContext.Categories.Find(id);
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
