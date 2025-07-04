using CinemaHub.Models;

namespace CinemaHub.Repositories.IRepositories
{
    public interface ICinemaRepository : IRepository<Cinema>
    {
        Task<int> GetTotalCountAsync();
    }
}
