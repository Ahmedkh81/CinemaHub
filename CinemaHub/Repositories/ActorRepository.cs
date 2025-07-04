using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        public ActorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<SelectListItem>> GetActorSelectListAsync(List<int>? selectedIds = null)
        {
            return await _context.Actors.Select(e => new SelectListItem
            {
                Text = e.FirstName + " " + e.LastName,
                Value = e.Id.ToString(),
                Selected = selectedIds != null && selectedIds.Contains(e.Id)
            }).ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Actors.CountAsync();
        }

        public async Task<List<Actor>> GetAllWithMoviesAsync()
        {
            return await _context.Actors
                .Include(e => e.MovieActors)
                .ThenInclude(e => e.Movie)
                .ToListAsync();
        }
        
    }
}
