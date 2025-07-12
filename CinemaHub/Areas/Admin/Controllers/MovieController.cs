using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using CinemaHub.Utility;
using CinemaHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private IMovieRepository _movieRepository;
        private ICategoryRepository _categoryRepository;
        private ICinemaRepository _cinemaRepository;
        private IActorRepository _actorRepository;
        private IMovieActorsRepository _movieActorsRepository;

        public MovieController(IMovieRepository movieRepository, ICategoryRepository categoryRepository,
            ICinemaRepository cinemaRepository, IActorRepository actorRepository
            , IMovieActorsRepository movieActorsRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieActorsRepository = movieActorsRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var movies = await _movieRepository.GetAsync(includes : new Expression<Func<Movie, object>>[]
            {
                e => e.Cinema,
                e => e.Category
            });

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

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAsync();
            var cinemas = await _cinemaRepository.GetAsync();
            var actors = await _actorRepository.GetActorSelectListAsync();

            MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
            {
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Movie = new Movie(),
                Actors = actors
            };
            return View(vm);
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageUrl, List<int> SelectedActorIds)
        {
            ModelState.Remove("ImageUrl");
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAsync();
                var cinemas = await _cinemaRepository.GetAsync();
                var actors = await _actorRepository.GetActorSelectListAsync();

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

            await _movieRepository.CreateAsync(movie);

            if (SelectedActorIds != null)
            {
                foreach (var actorId in SelectedActorIds)
                {
                    var movieActor = new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    };
                    await _movieActorsRepository.CreateAsync(movieActor);
                }
            }

            TempData["success-notification"] = "Movie Added Successfully";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Edit(int id)
        {
            //var movie = _context.Movies.Include(e => e.MovieActors).FirstOrDefault(e => e.Id == id);
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, includes: new Expression<Func<Movie, object>>[]
            {
                e => e.MovieActors
            });

            if (movie is not null)
            {
                var categories = await _categoryRepository.GetAsync();
                var cinemas = await _cinemaRepository.GetAsync();

                var selectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

                var actors = await _actorRepository.GetActorSelectListAsync(selectedActorIds);


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

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile ImageUrl)
        { 
            ModelState.Remove("ImageUrl");
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAsync();
                var cinemas = await _cinemaRepository.GetAsync();

                var selectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

                var actors = await _actorRepository.GetActorSelectListAsync(selectedActorIds);


                MovieWithCategoryWithCinemaVM vm = new MovieWithCategoryWithCinemaVM()
                {
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                    Movie = movie,
                    Actors = actors
                };
                return View(vm);
            }

            var movieImage = await _movieRepository.GetOneAsync(e => e.Id == movie.Id, tracked : false);
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
                await _movieRepository.UpdateAsync(movie);

                var selectedActorIds = Request.Form["Actors"].Select(int.Parse).ToList();

                var existingMovieActors = await _movieActorsRepository.GetAsync(e => e.MovieId == movie.Id);
                _movieActorsRepository.RemoveRange(existingMovieActors);

                foreach (var actorId in selectedActorIds)
                {
                    await _movieActorsRepository.CreateAsync(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }

                TempData["success-notification"] = "Movie Updated Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie is not null)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", movie.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                await _movieRepository.DeleteAsync(movie);

                TempData["success-notification"] = "Movie Deleted Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
