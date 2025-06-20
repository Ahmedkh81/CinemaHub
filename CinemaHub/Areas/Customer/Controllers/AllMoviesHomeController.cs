using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AllMoviesHomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int id)
        {
            var result = _context.Movies.Include(e => e.Category)
                .Where(e => e.Category.Id == id);
            return View(result.ToList());
        }
    }
}
