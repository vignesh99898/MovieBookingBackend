using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Customer
{
    [Table("CustomerDetails")]
    public class CustomerDetails
    {
        [Key]
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public long PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
    }
}