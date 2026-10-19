using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Project.ViewModels
{
    public class ResetPasswordVM
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        [DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
