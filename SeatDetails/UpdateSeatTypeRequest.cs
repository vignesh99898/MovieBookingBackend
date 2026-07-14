using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Seat
{
    public class UpdateSeatTypeRequest
    {
        [Required]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        public string RowName { get; set; } = string.Empty;

        [Required]
        public string SeatType { get; set; } = string.Empty;
    }
}