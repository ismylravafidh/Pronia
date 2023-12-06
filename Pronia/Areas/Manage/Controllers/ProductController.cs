using Microsoft.EntityFrameworkCore;
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
            List<Product> product = _dbContext.Products.Where(p => p.IsPrime == false).Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag).Include(p => p.ProductImages).ToList();
            return View(product);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Tags = await _dbContext.Tags.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm createProductVm)
        {
            ViewBag.Categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Tags = await _dbContext.Tags.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }
            bool resultCategory = await _dbContext.Categories.AnyAsync(c => c.Id == createProductVm.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category Movcud deyil");
                return View();
            }
            Product product = new Product()
            {
                Name = createProductVm.Name,
                Price = createProductVm.Price,
                Description = createProductVm.Description,
                SKU = createProductVm.SKU,
                CategoryId = createProductVm.CategoryId,
                ProductImages = new List<ProductImage>()
            };
            if (createProductVm.TagIds != null)
            {
                foreach (int tagId in createProductVm.TagIds)
                {
                    bool resultTag = await _dbContext.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", $"{tagId}-idli Tag Movcud deyil");
                        return View();
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        Product = product,
                        TagId = tagId,
                    };

                    _dbContext.ProductTags.Add(productTag);

                }

            }

            
            
            
            

            ProductImage mainImage = new ProductImage()
            {
                IsPrime = true,
                ImgUrl = createProductVm.MainPhoto.Upload(_environment.WebRootPath, @"\Upload\ProductImage\"),
                Product = product,
            };
            ProductImage hoverImage = new ProductImage()
            {
                IsPrime = false,
                ImgUrl = createProductVm.HoverPhoto.Upload(_environment.WebRootPath, @"\Upload\ProductImage\"),
                Product = product,
            };
            TempData["Error"] = "";
            product.ProductImages.Add(mainImage);
            product.ProductImages.Add(hoverImage);
            if (createProductVm.Photos != null)
            {
                foreach (var item in createProductVm.Photos)
                {
                    
                    ProductImage newPhoto = new ProductImage()
                    {
                        IsPrime = null,
                        ImgUrl = item.Upload(_environment.WebRootPath, @"\Upload\ProductImage\"),
                        Product = product,
                    };
                    product.ProductImages.Add(newPhoto);
                }
            }
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            Product product = await _dbContext.Products.Where(p => p.IsPrime == false).Include(p => p.Category)
                .Include(p => p.ProductTags)
                .ThenInclude(p => p.Tag)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            if (product is null)
            {
                return View("Error");
            }
            ViewBag.Categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Tags = await _dbContext.Tags.ToListAsync();
            UpdateProductVm updateProductVm = new UpdateProductVm()
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                TagIds = new List<int>(),
                productImages = new List<ProductImagesVm>()
            };

            foreach (var item in product.ProductTags)
            {
                updateProductVm.TagIds.Add((int)item.TagId);
            }
            foreach (var item in product.ProductImages)
            {
                ProductImagesVm productImages = new ProductImagesVm()
                {
                    IsPrime = item.IsPrime,
                    ImgUrl = item.ImgUrl,
                    Id = item.Id
                };
                updateProductVm.productImages.Add(productImages);
            }

            return View(updateProductVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVm updateProductVm)
        {
            ViewBag.Categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Tags = await _dbContext.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            Product existProduct = await _dbContext.Products.Where(p => p.IsPrime == false).Include(p => p.ProductImages).Include(p => p.ProductTags).Where(p => p.Id == updateProductVm.Id).FirstOrDefaultAsync();
            if (existProduct is null)
            {
                return View("Error");
            }
            bool resultCategory = await _dbContext.Categories.AnyAsync(c => c.Id == updateProductVm.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category Movcud deyil");
                return View();
            }
            existProduct.Name = updateProductVm.Name;
            existProduct.Description = updateProductVm.Description;
            existProduct.Price = updateProductVm.Price;
            existProduct.SKU = updateProductVm.SKU;
            existProduct.CategoryId = updateProductVm.CategoryId;

            if (updateProductVm.TagIds != null)
            {

                foreach (int tagId in updateProductVm.TagIds)
                {
                    bool resultTag = await _dbContext.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", $"{tagId}-idli Tag Movcud deyil");
                        return View();
                    }
                }
                List<int> createTags;
                if (existProduct.ProductTags != null)
                {
                    createTags = updateProductVm.TagIds.Where(ti => !existProduct.ProductTags.Exists(pt => pt.TagId == ti)).ToList();
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
                        ProductId = existProduct.Id
                    };
                    await _dbContext.ProductTags.AddAsync(productTag);

                }

                List<ProductTag> removeTags = existProduct.ProductTags.Where(pt => !updateProductVm.TagIds.Contains((int)pt.TagId)).ToList();

                _dbContext.ProductTags.RemoveRange(removeTags);

            }
            else
            {
                var productTagList = _dbContext.ProductTags.Where(pt => pt.ProductId == existProduct.Id).ToList();
                _dbContext.ProductTags.RemoveRange(productTagList);
            }
            TempData["Error"] = "";
            if (updateProductVm.MainPhoto != null)
            {
                
                ProductImage newMainImages = new ProductImage()
                {
                    IsPrime = true,
                    ProductId = existProduct.Id,
                    ImgUrl = updateProductVm.MainPhoto.Upload(_environment.WebRootPath, @"\Upload\ProductImage\")
                };
                var oldmainPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == true);
                existProduct.ProductImages?.Remove(oldmainPhoto);
                existProduct.ProductImages.Add(newMainImages);

            }
            if (updateProductVm.HoverPhoto != null)
            {
                
                ProductImage newHoverImages = new ProductImage()
                {
                    IsPrime = false,
                    ProductId = existProduct.Id,
                    ImgUrl = updateProductVm.HoverPhoto.Upload(_environment.WebRootPath, @"\Upload\ProductImage\")
                };
                var oldHoverPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == false);
                existProduct.ProductImages?.Remove(oldHoverPhoto);
                existProduct.ProductImages.Add(newHoverImages);
            }
            if (updateProductVm.Photos != null)
            {
                foreach (var item in updateProductVm.Photos)
                {
                    
                    ProductImage newPhoto = new ProductImage()
                    {
                        IsPrime = null,
                        ImgUrl = item.Upload(_environment.WebRootPath, @"\Upload\ProductImage\"),
                        Product = existProduct,
                    };
                    existProduct.ProductImages.Add(newPhoto);
                }
            }
            if (updateProductVm.ImageIds != null)
            {
                var removeListImage = existProduct.ProductImages.Where(p => !updateProductVm.ImageIds.Contains(p.Id) && p.IsPrime == null).ToList();
                foreach (var item in removeListImage)
                {
                    existProduct.ProductImages.Remove(item);
                    item.ImgUrl.DeleteFile(_environment.WebRootPath, @"\Upload\ProductImage\");
                }
            }
            else
            {
                existProduct.ProductImages.RemoveAll(p => p.IsPrime == null);
            }



            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.Where(p => p.IsPrime == false).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return View("Error");
            }
            product.IsPrime = true;
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
