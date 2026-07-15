using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.ShowTime
{
    public class ShowTimeRequest
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public string TheatreName { get; set; } = string.Empty;

        [Required]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        public List<ShowTiming> ShowTimings { get; set; } = new List<ShowTiming>();
    }

    public class ShowTiming
    {
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}