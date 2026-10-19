namespace E_Commerce_Project.Repositories
{
    public class ProductColorsRepository : Repository<ProductColors>, IProductColorsRepository
    {

        public ProductColorsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<ProductColors> productColors) 
        {
            _context.ProductColors.RemoveRange(productColors);
        }

    }
}
