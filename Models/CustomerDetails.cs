using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Models
{
    [Table("customerdetails")]
    public class CustomerDetails
    {
        [Key]
        [Column("userid")]
        public int UserId { get; set; }

        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        [Column("PhoneNumber")]
        public long PhoneNumber { get; set; }
    }
}