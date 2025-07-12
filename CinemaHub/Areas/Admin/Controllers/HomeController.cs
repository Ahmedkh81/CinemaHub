using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using CinemaHub.Utility;
using CinemaHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin},{Sd.Employee},{Sd.Company}")]
    public class HomeController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IActorRepository _actorRepository;

        public HomeController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository
            , IActorRepository actorRepository)
        {
            _movieRepository = movieRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> Index()
        {
            var totalMovies = await _movieRepository.GetTotalCountAsync();
            var totalCinemas = await _cinemaRepository.GetTotalCountAsync();
            var totalActors = await _actorRepository.GetTotalCountAsync();
            var actors = await _actorRepository.GetAllWithMoviesAsync();


            var availableMovies = await _movieRepository.GetCountByStatusAsync(MovieStatus.Available);
            var upComingMovies = await _movieRepository.GetCountByStatusAsync(MovieStatus.Upcoming);
            var expiredMovies = await _movieRepository.GetCountByStatusAsync(MovieStatus.Expired);

            var vm = new DashBoardAdminVM
            {
                TotalMovies = totalMovies,
                TotalCinemas = totalCinemas,
                TotalActors = totalActors,
                AvailableMovies = availableMovies,
                UpcomingMovies = upComingMovies,
                ExpiredMovies = expiredMovies,
                Actors = actors
            };
            return View(vm);
        }
    }
}
