using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Project.Models
{
    public class Promotion
    {
        public int Id { get; set; } 
        public int  ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public string Code { get; set; }
        public decimal Discount { get; set; }
        public int MaxUsage { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
