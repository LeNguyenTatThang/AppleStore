using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; } // Cho phép NULL

        [StringLength(15)]
        public string? PhoneNumber { get; set; } // Cho phép NULL

        [StringLength(20)]
        public string? Role { get; set; } = "User";

        [StringLength(20)]
        public string? Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } // Cho phép NULL

        public string? ResetPasswordToken { get; set; } // Cho phép NULL

        public DateTime? ResetTokenExpires { get; set; } // Cho phép NULL
    }

}
