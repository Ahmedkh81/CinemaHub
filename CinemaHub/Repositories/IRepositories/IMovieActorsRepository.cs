using CinemaHub.Models;

namespace CinemaHub.Repositories.IRepositories
{
    public interface IMovieActorsRepository : IRepository<MovieActor>
    {
        Task AddActorsToMovieAsync(int movieId, List<int> actorIds);
    }
}
