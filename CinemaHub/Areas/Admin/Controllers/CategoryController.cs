using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using CinemaHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync();
            return View(categories);
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.CreateAsync(category);

                TempData["success-notification"] = "Category Added Successfully";

                return RedirectToAction("Index");
            }

            return View();
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e=>e.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);

                TempData["success-notification"] = "Category Updated Successfully";

                return RedirectToAction("Index");
            }

            return View();
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category is not null)
            {
                await _categoryRepository.DeleteAsync(category);
                TempData["success-notification"] = "Category Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
