using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var categories = _context.Categories;
            return View(categories.ToList());
        }
    }
}
