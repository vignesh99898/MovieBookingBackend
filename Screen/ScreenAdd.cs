using System.ComponentModel.DataAnnotations;
namespace MovieBookingBackend.Screen
{
    public class ScreenAdd
    {
        public string TheatreName { get; set; } = string.Empty;

        public string ScreenName { get; set; } = string.Empty;

        public int TotalSeats { get; set; }
    }
}