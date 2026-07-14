using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Theatre
{
    public class ActiveTheatre
    {
        [Required]
        public string TheatreName { get; set; } = string.Empty;
    }
} 