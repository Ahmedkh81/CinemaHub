using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
