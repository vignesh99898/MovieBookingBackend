using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Booking
{
    public class BookingSeatRequest
    {
        [Required]
        public string SeatNumber { get; set; } = string.Empty;
    }
}