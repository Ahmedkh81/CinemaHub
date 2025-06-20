using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var totalMovies = _context.Movies.Count();
            var totalCinemas = _context.Cinemas.Count();
            var totalActors = _context.Actors.Count();
            var actors = _context.Actors.Include(e => e.Movies);
            

            var availableMovies = _context.Movies.AsEnumerable().Count(e => e.MovieStatus == MovieStatus.Available);
            var upComingMovies = _context.Movies.AsEnumerable().Count(e => e.MovieStatus == MovieStatus.Upcoming);
            var expiredMovies = _context.Movies.AsEnumerable().Count(e => e.MovieStatus == MovieStatus.Expired);

            var vm = new DashBoardAdminVM
            {
                TotalMovies = totalMovies,
                TotalCinemas = totalCinemas,
                TotalActors = totalActors,
                AvailableMovies = availableMovies,
                UpcomingMovies = upComingMovies,
                ExpiredMovies = expiredMovies,
                Actors = actors.ToList()
            };
            return View(vm);
        }
    }
}
