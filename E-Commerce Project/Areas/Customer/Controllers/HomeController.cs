using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_Commerce_Project.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;//new ApplicationDbContext();
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            products = products.Skip(0).Take(8);
            return View(products.AsEnumerable());
        }

        public IActionResult Privacy()
        {
            return View();
        }




        public IActionResult ProductDetails(int id)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            var raltedproducts = products.Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                                            .Skip(0)
                                            .Take(4);

            return View(new ProductWithRelatedVM()
            {
                Product = product,
                RelatedProducts = raltedproducts.AsEnumerable()
            });
        }














        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
