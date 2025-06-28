using CinemaHub.Data;
using CinemaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var cinemas = _context.Cinemas;
            return View(cinemas.ToList());
        }

        public IActionResult Create()
        {
            return View(new Cinema());
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Cinemas.Add(cinema);
            _context.SaveChanges();

            TempData["success-notification"] = "Cinema Added Successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();

            TempData["success-notification"] = "Cinema Updated Successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
            {
                return NotFound();
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            TempData["success-notification"] = "Cinema Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
