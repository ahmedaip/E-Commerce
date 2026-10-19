using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.WebRequestMethods;

namespace E_Commerce_Project.Models
{
    [Index(nameof(OTP), IsUnique = true)]
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsValid { get; set; }
        public ApplicationUserOTP()
        {
        }

        public ApplicationUserOTP(string userId, string otp)
        {
            Id = Guid.NewGuid().ToString();
            ApplicationUserId = userId;
            OTP = otp;
            IsValid = true;
            CreatedAt = DateTime.UtcNow;
            ValidTo = DateTime.UtcNow.AddMinutes(10);
        }


    }
}
