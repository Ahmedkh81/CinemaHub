using CinemaHub.Data;
using CinemaHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string? search, int page = 1)
        {
            IQueryable<Movie> movies = _context.Movies.Include(e => e.Category).Include(e => e.Cinema);

            if (search is not null)
            {
                movies = movies.Where(e => e.Name.Contains(search));
            }
            ViewBag.Search = search;

            if (!movies.Any())
                return View("NotFound");

            var totalMoviesInPage = 6;
            var totalPages = Math.Ceiling((double)movies.Count() / totalMoviesInPage);

            if (totalPages < page)
                return View("NotFound");
            movies = movies
                .Skip((page - 1) * (int)totalMoviesInPage)
                .Take((int)totalMoviesInPage);
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(movies.ToList());
        }
    }
}
