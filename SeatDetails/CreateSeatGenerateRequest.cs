using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Seat
{
    public class SeatGenerateRequest
    {
        [Required]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        public int NumberOfRows { get; set; }

        [Required]
        public int SeatsPerRow { get; set; }
    }
}