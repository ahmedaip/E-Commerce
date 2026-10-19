using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce_Project.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Models.Product> _productRepository;
        private readonly IRepository<Promotion> _promotionRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Order> orderRepository, IRepository<Models.Product> productRepository, IRepository<Promotion> promotionRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string? code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            if (code != null)
            {
                var promotion = await _promotionRepository.GetOneAsync(p =>
                    p.Code == code &&
                    p.IsValid &&
                    p.ValidTo > DateTime.UtcNow &&
                    p.MaxUsage > 0
                );
                if (promotion != null)
                {
                    var cartInDb = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id && c.ProductId == promotion.ProductId);
                    if (cartInDb != null)
                    {
                        cartInDb.Price -= cartInDb.Price * promotion.Discount / 100;
                        promotion.MaxUsage--;
                        await _cartRepository.CommitAsync();
                        TempData["Success_Notification"] = "Promotion Applied Successfully";
                    }
                    else
                    {
                        TempData["Error_Notification"] = "There is no Product associated with this PromoCode";

                    }
                }
                else
                {
                    TempData["Error_Notification"] = "Invalid or expired PromotionCode";

                }
            }

            var carts = await _cartRepository.GetAllAsync(c => c.ApplicationUserId == user.Id, includes: [e => e.Product]);

            return View(carts);
        }


        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (product is null) return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb != null)
            {
                cartInDb.Count += count;
                await _cartRepository.CommitAsync();
            }
            else
            {
                var cart = new Cart()
                {
                    ProductId = productId,
                    ApplicationUserId = user.Id,
                    Count = count,
                    Price = product.Price - (product.Price * product.Discount / 100),
                };
                await _cartRepository.CreateAsync(cart);
                await _cartRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            if (cartInDb.Count > 1)
            {
                cartInDb.Count--;
            }

            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> IncrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (cartInDb.Count < product.Quantity)
                cartInDb.Count++;

            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteProductCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            _cartRepository.Delete(cartInDb);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var userCarts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Product]);

            var order = new Order()
            {
                ApplicationUserId = user.Id,
                TotalPrice = userCarts.Sum(c => c.Price * c.Count),

            };
            await _orderRepository.CreateAsync(order);
            await _orderRepository.CommitAsync();


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel?orderId={order.Id}",
            };
            foreach (var item in userCarts)
            {
                var sessionLineOption = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        },
                        UnitAmount = (long)(item.Price * 100),
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineOption);
            }
            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            await _orderRepository.CommitAsync();
            return Redirect(session.Url);


        }

    }
}
