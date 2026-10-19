using E_Commerce_Project.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        //[MinaMaxLengthAttribute(3, 20)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
