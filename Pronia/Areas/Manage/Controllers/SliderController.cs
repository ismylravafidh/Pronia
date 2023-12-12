using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pronia.Helpers;
using Pronia.Models;
using System.Net;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
		AppDbContext _dbContext;
		private readonly IWebHostEnvironment _environment;

		public SliderController(AppDbContext context, IWebHostEnvironment environment)
		{
			_dbContext = context;
			_environment = environment;
		}
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
			List<Slider> sliderList = _dbContext.Sliders.ToList();
			return View(sliderList);
		}
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Slider slider)
		{
			if (!slider.ImageFile.ContentType.Contains("image"))
			{
				ModelState.AddModelError("ImageFile", "Yalnizca Sekil yukluye bilersiz");
				return View();
			}
			if (slider.ImageFile.Length > 3170304)
			{
				ModelState.AddModelError("ImageFile", "Maxsimum 3mb yukluye bilersiz!!");
				return View();
			}
			slider.ImgUrl = slider.ImageFile.Upload(_environment.WebRootPath, @"\Upload\SliderImage\");
			if (!ModelState.IsValid)
			{
				return View();
			}

			_dbContext.Sliders.AddAsync(slider);
			_dbContext.SaveChangesAsync();
			return RedirectToAction("Index");
		}
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
		{
			Slider slider = _dbContext.Sliders.Find(id);
			return View(slider);
		}
		[HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Slider newSlider)
		{
			if (!newSlider.ImageFile.ContentType.Contains("image"))
			{
				ModelState.AddModelError("ImageFile", "Yalnizca Sekil yukluye bilersiz");
				return View();
			}
			if (newSlider.ImageFile.Length > 3170304)
			{
				ModelState.AddModelError("ImageFile", "Maxsimum 3mb yukluye bilersiz!!");
				return View();
			}
			newSlider.ImgUrl = newSlider.ImageFile.Upload(_environment.WebRootPath, @"\Upload\SliderImage\");
			Slider oldSlider = _dbContext.Sliders.Find(newSlider.Id);
			FileManager.DeleteFile(oldSlider.ImgUrl, _environment.WebRootPath, @"\Upload\SliderImage\");
			oldSlider.Title = newSlider.Title;
			oldSlider.SubTitle = newSlider.SubTitle;
			oldSlider.Description = newSlider.Description;
			oldSlider.ImageFile = newSlider.ImageFile;
			oldSlider.ImgUrl = newSlider.ImgUrl;
			_dbContext.SaveChanges();
			return RedirectToAction("Index");
		}
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
		{
			Slider slider = _dbContext.Sliders.FirstOrDefault(s => s.Id == id);

			_dbContext.Sliders.Remove(slider);
			_dbContext.SaveChanges();
			FileManager.DeleteFile(slider.ImgUrl, _environment.WebRootPath, @"\Upload\SliderImage\");
			return RedirectToAction("Index");
		}
	}
}
