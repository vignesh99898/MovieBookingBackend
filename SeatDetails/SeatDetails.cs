using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Seat
{
    public class SeatDetails
    {
        [Key]
        public int SeatId { get; set; }

        public int ScreenId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string SeatType { get; set; } = "Regular";
    }
}