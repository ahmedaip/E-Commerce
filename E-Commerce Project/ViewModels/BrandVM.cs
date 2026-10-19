namespace E_Commerce_Project.ViewModels
{
    public class BrandVM
    {
        public IEnumerable<Brand> Brands { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
