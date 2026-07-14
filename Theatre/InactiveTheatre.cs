using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Theatre
{
    public class InactiveTheatre
    {
        [Required]
        public string TheatreName { get; set; } = string.Empty;
    }
}