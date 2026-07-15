using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSeatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserSeatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAvailableSeats")]
        public IActionResult GetAvailableSeats(int showId)
        {
            try
            {
                // Find Show
                var show = _context.ShowTimeDetails
                    .FirstOrDefault(s => s.ShowId == showId);

                if (show == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Show not found."
                    });
                }

                // Get all seats for this screen
                var seats = _context.SeatDetails
                    .Where(s => s.ScreenId == show.ScreenId)
                    .ToList();

                // Get all BookingIds for this show
                var bookingIds = _context.BookingDetails
                    .Where(b => b.ShowId == showId &&
                                b.BookingStatus == "Booked")
                    .Select(b => b.BookingId)
                    .ToList();

                // Get booked SeatIds
                var bookedSeatIds = _context.BookingSeatDetails
                    .Where(bs => bookingIds.Contains(bs.BookingId))
                    .Select(bs => bs.SeatId)
                    .ToList();

                // Remove booked seats
                var availableSeats = seats
                    .Where(s => !bookedSeatIds.Contains(s.SeatId))
                    .Select(s => new
                    {
                        s.SeatId,
                        s.SeatNumber,
                        s.SeatType
                    })
                    .ToList();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Available seats retrieved successfully.",
                    Data = availableSeats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}