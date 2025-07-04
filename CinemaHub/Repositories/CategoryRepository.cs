using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;

namespace CinemaHub.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
