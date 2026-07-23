using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Feedback
{
    public class FeedbackAdd
    {
        [Required]
        public required string MovieName { get; set; }

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public TimeSpan ShowTime { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Review { get; set; }
    }
}