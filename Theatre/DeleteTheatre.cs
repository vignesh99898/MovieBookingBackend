using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Theatre
{
    public class DeleteTheatre
    {
        [Required]
        public string TheatreName { get; set; } = string.Empty;
    }
}