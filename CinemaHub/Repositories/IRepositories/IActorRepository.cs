using CinemaHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaHub.Repositories.IRepositories
{
    public interface IActorRepository : IRepository<Actor>
    {
        Task<List<SelectListItem>> GetActorSelectListAsync(List<int>? selectedIds = null);
        Task<int> GetTotalCountAsync();
        Task<List<Actor>> GetAllWithMoviesAsync();
    }
}
