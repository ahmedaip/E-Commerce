using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Project.ViewModels
{
    public class CreateBrandVM
    {
        [MaxLength(30)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public IFormFile ImageFile {  get; set; }
    }
}
