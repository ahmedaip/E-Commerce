using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commerce_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IProductSubImagesRepository _productSubImagesRepository;
        private readonly IProductColorsRepository _productColorsRepository;
        private ProductService _productService = new ProductService();

        public ProductController(IRepository<Product> productRepository, IRepository<Brand> brandRepository, IRepository<Category> categoryRepository, IProductSubImagesRepository productSubImagesRepository, IProductColorsRepository productColorsRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _productSubImagesRepository = productSubImagesRepository;
            _productColorsRepository = productColorsRepository;
        }


        //public ProductController()
        //{
        //_context = new ApplicationDbContext();
        //_productRepository = new Repository<Product>();
        //_brandRepository = new Repository<Brand>();
        //_categoryRepository = new Repository<Category>();
        //_productSubImagesRepository = new ProductSubImagesRepository();
        //_productColorsRepository = new ProductColorsRepository();
        //_productService = new ProductService();
        //}
        public async Task<IActionResult> Index(ProductFilterVM productFilterVM, int page = 1)
        {
            //var products = _context.Products.Include(p => p.Category).Include(p => p.Brand).AsQueryable();
            var products = await _productRepository.GetAllAsync(includes: [p => p.Category, p => p.Brand]);
            if (productFilterVM.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(productFilterVM.ProductName.Trim()));
                ViewBag.Name = productFilterVM.ProductName;
            }

            if (productFilterVM.MinPrice > 0)
            {
                products = products.Where(p => p.Price >= productFilterVM.MinPrice);
                ViewBag.MinPrice = productFilterVM.MinPrice;
            }

            if (productFilterVM.MaxPrice > 0)
            {
                products = products.Where(p => p.Price <= productFilterVM.MaxPrice);
                ViewBag.MaxPrice = productFilterVM.MaxPrice;
            }
            if (productFilterVM.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == productFilterVM.CategoryId);
                ViewBag.CategoryId = productFilterVM.CategoryId;
            }
            if (productFilterVM.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == productFilterVM.BrandId);
                ViewBag.BrandId = productFilterVM.BrandId;
            }

            if (productFilterVM.IsLowQuantity)
            {
                products = products.OrderBy(P => P.Quantity);
                ViewBag.IsLowQuantity = productFilterVM.IsLowQuantity;
            }
            //ViewData["Categories"] = _context.Categories.AsEnumerable();
            ViewData["Categories"] = await _categoryRepository.GetAllAsync();
            //ViewData["Brands"] = _context.Brands.AsEnumerable();
            ViewData["Brands"] = await _brandRepository.GetAllAsync();

            int totalpages = (int)Math.Ceiling(products.Count() / 5.0);
            products = products.Skip((page - 1) * 5).Take(5);

            return View(new ProductVM()
            {
                Products = products,
                TotalPages = totalpages,
                CurrentPage = page,
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var categories = _context.Categories.AsEnumerable();
            var categories = await _categoryRepository.GetAllAsync();
            //var brands = _context.Brands.AsEnumerable();
            var brands = await _brandRepository.GetAllAsync();

            return View(new CreateUpdateVM()
            {
                Categories = categories,
                Brands = brands
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _productService.SaveFile(ImageFile);

                product.MainImg = filename;
            }
            //var savedproduct = _context.Products.Add(product);
            var savedproduct = await _productRepository.CreateAsync(product);
            //_context.SaveChanges();
            await _productRepository.CommitAsync();

            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                foreach (var image in SubImageFiles)
                {
                    var filename = _productService.SaveFile(image, ProductImageType.SubImage);
                    //_context.ProductSubImages.Add(new ProductSubImages()
                    await _productSubImagesRepository.CreateAsync(new ProductSubImages()
                    {
                        ProductId = savedproduct.Entity.Id,
                        Img = filename
                    });
                }
                //_context.SaveChanges();
                await _productSubImagesRepository.CommitAsync();
            }
            if (Colors != null && Colors.Count > 0)
            {
                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColors()
                    await _productColorsRepository.CreateAsync(new ProductColors()
                    {
                        ProductId = savedproduct.Entity.Id,
                        Color = color
                    });
                }
                //_context.SaveChanges();
                await _productColorsRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepository.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            //var categories = _context.Categories.AsEnumerable();
            var categories = await _categoryRepository.GetAllAsync();
            //var brands = _context.Brands.AsEnumerable();
            var brands = await _brandRepository.GetAllAsync();
            return View(new CreateUpdateVM()
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                //ProductSubImages = _context.ProductSubImages.Where(ps => ps.ProductId == id),
                ProductSubImages = await _productSubImagesRepository.GetAllAsync(ps => ps.ProductId == id),
                //ProductColors = _context.ProductColors.Where(pc => pc.ProductId == id)
                ProductColors = await _productColorsRepository.GetAllAsync(pc => pc.ProductId == id),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
        {
            //var productIndb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productIndb = await _productRepository.GetOneAsync(b => b.Id == product.Id, IsTracking: false);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _productService.SaveFile(ImageFile);
                product.MainImg = filename;

                _productService.RemoveFile(productIndb.MainImg);
            }
            else
            {
                product.MainImg = productIndb.MainImg;
            }

            //_context.Products.Update(product);
            _productRepository.Update(product);
            //_context.SaveChanges();
            await _productRepository.CommitAsync();

            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {

                //var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
                var oldSubImages = await _productSubImagesRepository.GetAllAsync(p => p.ProductId == product.Id);
                //_context.ProductSubImages.RemoveRange(oldSubImages);
                _productSubImagesRepository.DeleteRange(oldSubImages);
                foreach (var item in oldSubImages)
                {
                    _productService.RemoveFile(item.Img, ProductImageType.SubImage);
                }

                foreach (var image in SubImageFiles)
                {
                    var filename = _productService.SaveFile(image, ProductImageType.SubImage);

                    //_context.ProductSubImages.Add(new ProductSubImages()
                    await _productSubImagesRepository.CreateAsync(new ProductSubImages()
                    {
                        ProductId = product.Id,
                        Img = filename
                    });
                }
                //_context.SaveChanges();
                await _productSubImagesRepository.CommitAsync();
            }

            if (Colors != null && Colors.Count > 0)
            {
                //var oldProductColor = _context.ProductColors.Where(pc => pc.ProductId == product.Id);
                var oldProductColor = await _productColorsRepository.GetAllAsync(pc => pc.ProductId == product.Id);
                //_context.ProductColors.RemoveRange(oldProductColor);
                _productColorsRepository.DeleteRange(oldProductColor);

                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColors()
                    await _productColorsRepository.CreateAsync(new ProductColors()
                    {
                        ProductId = product.Id,
                        Color = color
                    });
                }
                //_context.SaveChanges();
                await _productColorsRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepository.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                _productService.RemoveFile(product.MainImg);
                //_context.Products.Remove(product);
                _productRepository.Delete(product);
                //var productSubImages = _context.ProductSubImages.Where(ps => ps.ProductId == product.Id);
                var productSubImages = await _productSubImagesRepository.GetAllAsync(ps => ps.ProductId == product.Id);
                foreach (var image in productSubImages)
                {
                    _productService.RemoveFile(image.Img, ProductImageType.SubImage);
                }
                //_context.SaveChanges();
                await _productRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DeleteSubImage(int id)
        {
            //var productSubImage = _context.ProductSubImages.FirstOrDefault(c => c.Id == id);
            var productSubImage = await _productSubImagesRepository.GetOneAsync(c => c.Id == id);

            if (productSubImage is null)
            {
                return NotFound();
            }
            //_context.ProductSubImages.Remove(productSubImage);
            _productSubImagesRepository.Delete(productSubImage);
            _productService.RemoveFile(productSubImage.Img, ProductImageType.SubImage);
            //_context.SaveChanges();
            await _productSubImagesRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}