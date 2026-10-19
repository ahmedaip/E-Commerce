using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Project.Models
{
    public class ProductSubImages
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
