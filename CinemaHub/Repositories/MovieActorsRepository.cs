using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;

namespace CinemaHub.Repositories
{
    public class MovieActorsRepository : Repository<MovieActor>, IMovieActorsRepository
    {
        public MovieActorsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddActorsToMovieAsync(int movieId, List<int> actorIds)
        {
            var movieActors = actorIds.Select(actorId => new MovieActor
            {
                MovieId = movieId,
                ActorId = actorId
            });

            await _context.MovieActors.AddRangeAsync(movieActors);
        }
    }
}
