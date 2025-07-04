using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Movies.CountAsync();
        }

        public async Task<int> GetCountByStatusAsync(MovieStatus status)
        {
            return (await _context.Movies.ToListAsync())
           .Count(m => m.MovieStatus == status);
        }
    }
}
