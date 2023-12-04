using Pronia.Areas.ViewModels;
using Pronia.Helpers;
using Pronia.Models;
using System.Linq;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext context, IWebHostEnvironment environment)
        {
            _dbContext = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            List<Product> productList = _dbContext.Products.Include(p=>p.Category)
                .Include(p=>p.ProductTags).ThenInclude(p=>p.Tag).ToList();
            return View(productList);
        }
        public IActionResult Create()
        {
            ViewBag.Categories=_dbContext.Categories.ToList();
            ViewBag.Tags = _dbContext.Tags.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVm createProductVm)
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Tags = _dbContext.Tags.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }

            Product product = new Product()
            {
                Name= createProductVm.Name,
                SKU=createProductVm.SKU,
                Price=createProductVm.Price,
                Description=createProductVm.Description,
                CategoryId=createProductVm.CategoryId,
            };
            if (createProductVm.TagIds != null)
            {
                foreach(int tagId in createProductVm.TagIds)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        Product=product,
                        TagId=tagId
                    };
                    _dbContext.Add(productTag);
                }
            }


            //ProductImage productImage = new ProductImage();
            //productImage.MainImgUrl = productImage.MainImageFile.Upload(_environment.WebRootPath, @"\Upload\ProductImage\");
            //productImage.HoverImgUrl = productImage.HoverImageFile.Upload(_environment.WebRootPath, @"\Upload\ProductImage\");
            //productImage.AdditionImgUrl = productImage.AdditionImageFile.Upload(_environment.WebRootPath, @"\Upload\ProductImage\");
            //_dbContext.ProductImages.Add(productImage);
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Tags = _dbContext.Tags.ToList();
            Product product = _dbContext.Products.Include(P=>P.Category)
                .Include(p=>p.ProductTags).ThenInclude(p=>p.Tag).Where(p=>p.Id == id).FirstOrDefault();

            UpdateProductVm updateProductVm = new UpdateProductVm()
            {
                Id = id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                TagIds = new List<int>()
            };

            foreach(var item in product.ProductTags)
            {
                updateProductVm.TagIds.Add((int)item.TagId);
            }


            return View(updateProductVm);
        }
        [HttpPost]
        public IActionResult Update(UpdateProductVm updateProductVm)
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Tags = _dbContext.Tags.ToList();
            if(!ModelState.IsValid)
            {
                return View();
            }

            Product oldProduct = _dbContext.Products.Where(p=>p.Id==updateProductVm.Id).FirstOrDefault();

            oldProduct.Name = updateProductVm.Name;
            oldProduct.Price = updateProductVm.Price;
            oldProduct.SKU = updateProductVm.SKU;
            oldProduct.Description = updateProductVm.Description;
            oldProduct.CategoryId = updateProductVm.CategoryId;
            if (updateProductVm.TagIds != null)
            {
                foreach (int tagId in updateProductVm.TagIds)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        Product = oldProduct,
                        TagId = tagId
                    };
                    _dbContext.Add(productTag);
                }
            }
            List<int> createTags;
            if (oldProduct.ProductTags != null)
            {
                createTags = updateProductVm.TagIds.Where(p => !oldProduct.ProductTags.Exists(pt => pt.TagId == p)).ToList();
            }
            else
            {
                createTags = updateProductVm.TagIds.ToList();
            }
            foreach (int tagId in createTags)
            {
                ProductTag productTag = new ProductTag()
                {
                    TagId = tagId,
                    ProductId = oldProduct.Id
                };
                _dbContext.ProductTags.Add(productTag);
            }
            List<ProductTag> RemoveTags = oldProduct.ProductTags.Where(p => updateProductVm.TagIds.Contains((int)p.TagId)).ToList();
            _dbContext.ProductTags.RemoveRange(RemoveTags);
            if(oldProduct.ProductTags == null)
            {
                var productTagList = _dbContext.ProductTags.Where(p => p.ProductId == oldProduct.Id).ToList();
                _dbContext.ProductTags.RemoveRange(productTagList);

            }

            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p=>p.Id==id);

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
