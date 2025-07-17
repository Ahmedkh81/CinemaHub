using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CreateRangeAsync(List<OrderItem> entities)
        {
            try
            {
                _context.OrderItems.AddRange(entities);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }
    }
}
