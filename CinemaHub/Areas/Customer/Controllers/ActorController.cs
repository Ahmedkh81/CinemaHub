using CinemaHub.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int id)
        {          
            var actor = _context.Actors.FirstOrDefault(e => e.Id == id);

            if (actor is null)
                return View("NotFound");
            return View(actor);
        }
    }
}
