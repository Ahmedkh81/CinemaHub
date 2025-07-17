using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DetailsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int id)
        {
            var movie = _context.Movies
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                .Include(e => e.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .Include(e => e.MovieSchedules)
                .FirstOrDefault(e => e.Id == id);

            if (movie is null)
                return View("NotFound");

            ViewBag.Schedules = movie.MovieSchedules.OrderBy(s => s.ShowTime).ToList();
            return View(movie);
        }
    }
}
