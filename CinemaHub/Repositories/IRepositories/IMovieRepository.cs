using CinemaHub.Models;

namespace CinemaHub.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<int> GetTotalCountAsync();
        Task<int> GetCountByStatusAsync(MovieStatus status);
    }
}
