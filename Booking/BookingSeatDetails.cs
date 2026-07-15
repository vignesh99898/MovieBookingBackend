using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Booking
{
    [Table("BookingSeatDetails")]
    public class BookingSeatDetails
    {
        [Key]
        public int BookingSeatId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int SeatId { get; set; }
    }
}