using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace E_Commerce_Project.Repositories
{
    public class ProductSubImagesRepository : Repository<ProductSubImages>, IProductSubImagesRepository
    {
        public ProductSubImagesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<ProductSubImages> productSubImages)
        {
            _context.ProductSubImages.RemoveRange(productSubImages);
        }
    }
}
