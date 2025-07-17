using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
