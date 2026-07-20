using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Booking
{
    public class BookingRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public string TheatreName { get; set; } = string.Empty;

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public List<BookingSeatRequest> Seats { get; set; } = new();
    }
}