using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Feedback
{
    [Table("FeedbackDetails")]
    public class FeedbackDetails
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Review { get; set; }

        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        // Foreign Keys

        [ForeignKey("BookingId")]
        public Booking.BookingDetails? BookingDetails { get; set; }

        [ForeignKey("UserId")]
        public Customer.CustomerDetails? CustomerDetails { get; set; }

        [ForeignKey("MovieId")]
        public Movie.MovieDetails? MovieDetails { get; set; }
    }
}