using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        public CinemaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Cinemas.CountAsync();
        }
    }
}
