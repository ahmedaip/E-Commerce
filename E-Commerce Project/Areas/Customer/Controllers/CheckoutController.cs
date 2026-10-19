using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace E_Commerce_Project.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Models.Product> _productRepository;

        public CheckoutController(IEmailSender emailSender, UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository, IRepository<Product> productRepository)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var order = await _orderRepository.GetOneAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            await _emailSender.SendEmailAsync(
                user.Email,
                "Payment",
                $"<h1>Payment Succeeded</h1>"
            );
            order.OrderStatus = OrderStatus.InProgress;
            order.PaymentStatus = PaymentStatus.Completed;
            await _orderRepository.CommitAsync();



            var userCarts = await _cartRepository.GetAllAsync(c => c.ApplicationUserId == user.Id, includes: [c => c.Product]);
            foreach (var item in userCarts)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Count = item.Count,

                };
                await _orderItemRepository.CreateAsync(orderItem);
                item.Product.Quantity -= item.Count;
                _cartRepository.Delete(item);
            }
            await _orderItemRepository.CommitAsync();


            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);
            order.TransactionId = session.PaymentIntentId;
            await _orderRepository.CommitAsync();
            return View();
        }

        public async Task<IActionResult> Cancel(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var order = await _orderRepository.GetOneAsync(o => o.Id == orderId);
            if (order == null) return NotFound();
            order.OrderStatus = OrderStatus.Canceled;
            await _orderRepository.CommitAsync();

            return View();
        }
    }
}
