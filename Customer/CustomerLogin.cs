using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Customer
{
    public class CustomerLogin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}