using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Project.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
