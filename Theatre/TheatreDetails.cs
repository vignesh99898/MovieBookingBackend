using System.ComponentModel.DataAnnotations;

public class TheatreDetails
{
    [Key]
    public int TheatreId { get; set; }

    public string TheatreName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";
}