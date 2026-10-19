using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Authorize(Roles = $"{CD.ROLE_SUPER_ADMIN}, {CD.ROLE_ADMIN}, {CD.ROLE_EMPLOYEE}")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string categoryName, int page = 1)
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAllAsync();
            if (categoryName != null)
            {
                categories = categories.Where(c => c.Name.Contains(categoryName));
                ViewBag.Name = categoryName;
            }
            int totalpages = (int)Math.Ceiling(categories.Count() / 3.0);
            categories = categories.Skip((page - 1) * 3).Take(3);

            return View(new CategoryVM()
            {
                Categories = categories,
                TotalPages = totalpages,
                CurrentPage = page,
            });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
            ;
        }


        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Add(category);
            await _categoryRepository.CreateAsync(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("Success_Notification", "Category Created Successfully");
            TempData["Success_Notification"] = "Category Created Successfully";
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = $"{CD.ROLE_SUPER_ADMIN}, {CD.ROLE_ADMIN}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [Authorize(Roles = $"{CD.ROLE_SUPER_ADMIN}, {CD.ROLE_ADMIN}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Update(category);
            _categoryRepository.Update(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        
        [Authorize(Roles = $"{CD.ROLE_SUPER_ADMIN}, {CD.ROLE_ADMIN}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
             var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                //_context.Categories.Remove(category);
                _categoryRepository.Delete(category);
                //_context.SaveChanges();
                await _categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));

            }
        }

    }
}