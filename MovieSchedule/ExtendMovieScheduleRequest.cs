using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.MovieSchedule
{
    public class ExtendMovieScheduleRequest
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public string TheatreName { get; set; } = string.Empty;

        [Required]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        public DateTime NewEndDate { get; set; }
    }
}