using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckOutController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMovieRepository _movieRepository;

        public CheckOutController(IOrderRepository orderRepository, ICartRepository cartRepository, IOrderItemRepository orderItemRepository
            , UserManager<ApplicationUser> userManager, IMovieRepository movieRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderItemRepository = orderItemRepository;
            _userManager = userManager;
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == orderId);

            if (order is null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.procession;

            var service = new SessionService();
            var session = service.Get(order.SessionId);

            order.PaymentId = session.PaymentIntentId;

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            List<OrderItem> orderItems = new();

            foreach (var item in carts)
            {
                orderItems.Add(new()
                {
                    MovieId = item.MovieId,
                    OrderId = orderId,
                    TotalPrice = item.Movie.Price * item.Count
                });
                var movie = await _movieRepository.GetOneAsync(e => e.Id == item.MovieId);

                if (movie is null)
                    return NotFound();

                movie.AvailableTickets -= item.Count;
                await _movieRepository.UpdateAsync(movie);
            }

            await _orderItemRepository.CreateRangeAsync(orderItems);

            foreach (var item in carts)
            {
                await _cartRepository.DeleteAsync(item);
            }

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

    }
}
