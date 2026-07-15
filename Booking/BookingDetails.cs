using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Booking
{
    [Table("BookingDetails")]
    public class BookingDetails
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ShowId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string BookingStatus { get; set; } = string.Empty;
    }
}