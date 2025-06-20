using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaDetailsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int id)
        {
            var cinema = _context.Cinemas.Include(e => e.Movies).FirstOrDefault(e => e.Id == id);

            if (cinema is null)
                return View("NotFound");
            return View(cinema);
        }
    }
}
