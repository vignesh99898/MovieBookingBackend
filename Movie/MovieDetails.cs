using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingBackend.Movie
{
    [Table("MovieDetails")]
    public class MovieDetails
    {
        [Key]
        public int MovieId { get; set; }

        public string MovieName { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Status { get; set; } = "Active";
    }
}