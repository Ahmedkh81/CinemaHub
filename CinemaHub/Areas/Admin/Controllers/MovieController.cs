using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int page = 1)
        {
            IQueryable<Movie> movies = _context.Movies.Include(e => e.Cinema).Include(e => e.Category);

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

        public IActionResult Create()
        {
            var categories = _context.Categories;
            var cinemas = _context.Cinemas;
            var actors = _context.Actors.Select(e => new SelectListItem
            {
                Text = e.FirstName + " " + e.LastName,
                Value = e.Id.ToString()
            }).ToList();

            MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
            {
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Movie = new Movie(),
                Actors = actors
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageUrl, List<int> SelectedActorIds)
        {
            ModelState.Remove("ImageUrl");
            if (!ModelState.IsValid)
            {
                var categories = _context.Categories;
                var cinemas = _context.Cinemas;
                var actors = _context.Actors.Select(e => new SelectListItem
                {
                    Text = e.FirstName + " " + e.LastName,
                    Value = e.Id.ToString()
                }).ToList();

                MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
                {
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                    Movie = new Movie(),
                    Actors = actors
                };
                return View(vm);
            }

            if (ImageUrl is not null && ImageUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images" , fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImageUrl.CopyToAsync(stream);
                }

                movie.ImageUrl = fileName;
            }

            _context.Movies.Add(movie);
            _context.SaveChanges();

            if (SelectedActorIds != null)
            {
                foreach (var actorId in SelectedActorIds)
                {
                    var movieActor = new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    };
                    _context.MovieActors.Add(movieActor);
                }
            }
            _context.SaveChanges();

            TempData["success-notification"] = "Movie Added Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Include(e => e.MovieActors).FirstOrDefault(e => e.Id == id);

            if (movie is not null)
            {
                var categories = _context.Categories;
                var cinemas = _context.Cinemas;

                var selectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

                var actors = _context.Actors
                    .Select(e => new SelectListItem
                    {
                        Text = e.FirstName + " " + e.LastName,
                        Value = e.Id.ToString(),
                        Selected = selectedActorIds.Contains(e.Id)
                    }).ToList();


                MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
                {
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                    Movie = movie,
                    Actors = actors
                };
                return View(vm);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile ImageUrl)
        { 
            ModelState.Remove("ImageUrl");
            if (!ModelState.IsValid)
            {
                var categories = _context.Categories;
                var cinemas = _context.Cinemas;

                var selectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

                var actors = _context.Actors
                    .Select(e => new SelectListItem
                    {
                        Text = e.FirstName + " " + e.LastName,
                        Value = e.Id.ToString(),
                        Selected = selectedActorIds.Contains(e.Id)
                    }).ToList();


                MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
                {
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                    Movie = movie,
                    Actors = actors
                };
                return View(vm);
            }

            var movieImage = _context.Movies.AsNoTracking().FirstOrDefault(e => e.Id == movie.Id);
            if (movieImage is not null)
            {
                if (ImageUrl is not null && ImageUrl.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await ImageUrl.CopyToAsync(stream);
                    }

                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", movieImage.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    movie.ImageUrl = fileName;
                }
                else
                {
                    movie.ImageUrl = movieImage.ImageUrl;
                }
                _context.Movies.Update(movie);
                _context.SaveChanges();

                var selectedActorIds = Request.Form["Actors"].Select(int.Parse).ToList();

                var existingMovieActors = _context.MovieActors.Where(ma => ma.MovieId == movie.Id);
                _context.MovieActors.RemoveRange(existingMovieActors);

                foreach (var actorId in selectedActorIds)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }

                _context.SaveChanges();

                TempData["success-notification"] = "Movie Updated Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public ActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie is not null)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", movie.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                _context.Movies.Remove(movie);
                _context.SaveChanges();
                TempData["success-notification"] = "Movie Deleted Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
