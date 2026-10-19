namespace E_Commerce_Project.ViewModels
{
    public class CreateUpdateVM
    {
        public Product Product { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<ProductSubImages> ProductSubImages { get; set; }
        public IEnumerable<ProductColors> ProductColors { get; set; }

    }
}
