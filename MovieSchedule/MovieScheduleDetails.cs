using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieBookingBackend.Movie;
using MovieBookingBackend.Screen;
using MovieBookingBackend.Theatre;

namespace MovieBookingBackend.MovieSchedule
{
    [Table("MovieSchedule")]
    public class MovieScheduleDetails
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public MovieDetails? Movie { get; set; }

        [Required]
        public int TheatreId { get; set; }

        [ForeignKey("TheatreId")]
        public TheatreDetails? Theatre { get; set; }

        [Required]
        public int ScreenId { get; set; }

        [ForeignKey("ScreenId")]
        public ScreenDetails? Screen { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; } = "Active";
    }
}