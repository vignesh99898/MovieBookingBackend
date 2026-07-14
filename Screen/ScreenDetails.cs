using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Screen
{
    [Table("ScreenDetails")]
    public class ScreenDetails
    {
        [Key]
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        public int TotalSeats { get; set; }
    }
}