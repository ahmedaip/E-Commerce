namespace E_Commerce_Project.Repositories
{
    public interface IProductSubImagesRepository: IRepository<ProductSubImages>
    {
        void DeleteRange(IEnumerable<ProductSubImages> productSubImages);

    }
}
