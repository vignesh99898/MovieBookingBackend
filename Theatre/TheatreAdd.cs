using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Theatre
{
    public class TheatreAdd
    {
        [Required]
        public string TheatreName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}