namespace E_Commerce_Project.ViewModels
{
    public class ProductVM
    {
        public IEnumerable<Product> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
