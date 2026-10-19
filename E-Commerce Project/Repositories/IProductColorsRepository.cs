namespace E_Commerce_Project.Repositories
{
    public interface IProductColorsRepository :IRepository<ProductColors>
    {
        void DeleteRange(IEnumerable<ProductColors> productColors);

    }
}
