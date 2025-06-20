using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var cinemas = _context.Cinemas;

            if (cinemas is null)
                return View("NotFound");

            return View(cinemas.ToList());
        }
    }
}
