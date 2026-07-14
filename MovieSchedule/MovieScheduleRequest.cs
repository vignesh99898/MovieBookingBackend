using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.MovieSchedule
{
    public class MovieScheduleRequest
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int NumberOfTheatres { get; set; }

        [Required]
        public List<TheatreScreenRequest> TheatreScreenDetails { get; set; } = new List<TheatreScreenRequest>();
    }
}