using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commerce_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Brand> _brandRepository;
        private BrandService _brandService = new BrandService();

        public BrandController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index(string brandName, int page = 1)
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _brandRepository.GetAllAsync();
            if (brandName != null)
            {
                brands = brands.Where(c => c.Name.Contains(brandName));
                ViewBag.Name = brandName;
            }
            int totalpages = (int)Math.Ceiling(brands.Count() / 3.0);
            brands = brands.Skip((page - 1) * 3).Take(3);

            return View(new BrandVM()
            {
                Brands = brands,
                TotalPages = totalpages,
                CurrentPage = page,
            });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateBrandVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandVM createBrandVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createBrandVM);
            }
            var brand = new Brand()
            {
                Name = createBrandVM.Name,
                Description = createBrandVM.Description,
                Status = createBrandVM.Status,
            };
            if (createBrandVM.ImageFile != null && createBrandVM.ImageFile.Length > 0)
            {
                var filename = _brandService.SaveFile(createBrandVM.ImageFile);

                brand.Logo = filename;
            }
            //_context.Brands.Add(brand);
            await _brandRepository.CreateAsync(brand);
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand, IFormFile ImageFile)
        {
            //var brandIndb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            var brandIndb = await _brandRepository.GetOneAsync(b => b.Id == brand.Id, IsTracking: false);


            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _brandService.SaveFile(ImageFile);
                brand.Logo = filename;

                _brandService.RemoveFile(brandIndb.Logo);
            }
            else
            {
                brand.Logo = brandIndb.Logo;
            }

            //_context.Brands.Update(brand);
            _brandRepository.Update(brand);
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            else
            {
                _brandService.RemoveFile(brand.Logo);
                //_context.Brands.Remove(brand);
                _brandRepository.Delete(brand);
                //_context.SaveChanges();
                await _brandRepository.CommitAsync();
                return RedirectToAction(nameof(Index));

            }
        }



    }
}