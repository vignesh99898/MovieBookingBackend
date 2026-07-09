using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Customer
{
    public class CustomerRegister
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public long PhoneNumber { get; set; }
    }
}