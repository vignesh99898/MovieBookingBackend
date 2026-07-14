using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Movie
{
    public class MovieAdd
    {
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public int Duration { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

    }
}