namespace E_Commerce_Project.ViewModels
{
    public class OrderVM
    {
        public IEnumerable<Order> Orders { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
