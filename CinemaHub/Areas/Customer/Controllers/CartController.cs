using CinemaHub.Models;
using CinemaHub.Repositories;
using CinemaHub.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public CartController(IMovieRepository movieRepository, ICartRepository cartRepository, UserManager<ApplicationUser> userManager, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _movieRepository = movieRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IActionResult> AddToCart(int movieId, int count, int scheduleId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                var movie = await _movieRepository.GetOneAsync(e=> e.Id == movieId);
                if (movie is not null && movie.AvailableTickets >= count && count > 0)
                {
                    var movieInCart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId &&
                        e.MovieScheduleId == scheduleId);


                    if (movieInCart is not null)
                    {
                        movieInCart.Count += count;
                        await _cartRepository.UpdateAsync(movieInCart);
                    }
                    else
                    {
                        await _cartRepository.CreateAsync(new Cart
                        {
                            ApplicationUserId = user.Id,
                            MovieId = movieId,
                            Count = count,
                            MovieScheduleId = scheduleId
                        });
                    }
                    TempData["success-notification"] = "Added to Cart successfully";
                    return RedirectToAction("Index", "Home");
                }
                TempData["error-notification"] = "the count of tickets is not availble";
                return RedirectToAction("Index", "Details", new { area = "Customer", id = movieId });
            }
            return NotFound();
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id
                , includes: [e => e.Movie, e=> e.MovieSchedule]);
                var totalPrice = carts.Sum(e => e.Movie.Price * e.Count);
                ViewBag.totalPrice = totalPrice;
                return View(carts);
            }
            return NotFound();
        }

        public async Task<IActionResult> IncrementCount(int movieId, int scheduleId)
        {
            var user = await _userManager.GetUserAsync(User);
            var AvailableTickets = await _movieRepository.GetOneAsync(e => e.Id == movieId);

            if (user is not null)
            {
                var movieInCart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId &&
                        e.MovieScheduleId == scheduleId);
                if (movieInCart is not null)
                {
                    if (movieInCart.Count < AvailableTickets.AvailableTickets)
                    { 
                        movieInCart.Count++;
                        await _cartRepository.UpdateAsync(movieInCart);
                    }

                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return NotFound();
        }

        public async Task<IActionResult> DecrementCount(int movieId, int scheduleId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var movieInCart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId &&
                        e.MovieScheduleId == scheduleId);
                if (movieInCart is not null)
                {
                    if (movieInCart.Count > 1)
                    {
                        movieInCart.Count--;
                        await _cartRepository.UpdateAsync(movieInCart);
                    }
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return NotFound();
        }

        public async Task<IActionResult> RemoveFromCart(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                var movieInCart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
                if (movieInCart is not null)
                {
                    await _cartRepository.DeleteAsync(movieInCart);
                    TempData["success-notification"] = "Removed from Cart successfully";
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return NotFound();
        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

                if (carts is not null)
                {
                    await _orderRepository.CreateAsync(new Order
                    {
                        ApplicationUserId = user.Id,
                        TotalPrice = carts.Sum(e => e.Movie.Price * e.Count),
                        Date = DateTime.Now,
                        OrderStatus = OrderStatus.pending,
                        PaymentMethod = PaymentMethod.Visa
                    });

                    var order = (await _orderRepository.GetAsync(e => e.ApplicationUserId == user.Id)).OrderBy(e => e.Id)
                        .LastOrDefault();

                    if (order is null)
                        return BadRequest();

                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                        CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
                    };


                    foreach (var item in carts)
                    {
                        options.LineItems.Add(new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "egp",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Movie.Name,
                                    Description = item.Movie.Description,
                                },
                                UnitAmount = (long)item.Movie.Price * 100,
                            },
                            Quantity = item.Count,
                        });
                    }


                    var service = new SessionService();
                    var session = service.Create(options);
                    order.SessionId = session.Id;
                    await _orderRepository.UpdateAsync(order);
                    return Redirect(session.Url);
                }
                return NotFound();
            }
            return NotFound();
        }
    }    
}
