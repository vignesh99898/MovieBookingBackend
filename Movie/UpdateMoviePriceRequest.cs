using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Movie
{
    public class UpdateMoviePriceRequest
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public decimal TicketPrice { get; set; }
    }
}