using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories;
using CinemaHub.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private ICinemaRepository _cinemaRepository;

        public CinemaController(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }
        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetAsync();
            return View(cinemas);
        }

        public IActionResult Create()
        {
            return View(new Cinema());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _cinemaRepository.CreateAsync(cinema);

            TempData["success-notification"] = "Cinema Added Successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (cinema is null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _cinemaRepository.UpdateAsync(cinema);

            TempData["success-notification"] = "Cinema Updated Successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (cinema is null)
            {
                return NotFound();
            }
            await _cinemaRepository.DeleteAsync(cinema);
            TempData["success-notification"] = "Cinema Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
