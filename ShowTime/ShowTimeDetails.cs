using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.Screen;

namespace MovieBookingBackend.ShowTime
{
    [Table("ShowTime")]
    public class ShowTimeDetails
    {
        [Key]
        public int ShowId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [ForeignKey("ScheduleId")]
        public MovieScheduleDetails? MovieSchedule { get; set; }

        [Required]
        public int ScreenId { get; set; }

        [ForeignKey("ScreenId")]
        public ScreenDetails? Screen { get; set; }

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int AvailableSeats { get; set; }
    }
}