using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Models;
using MovieBookingBackend.Seat;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("GenerateSeats")]
        public IActionResult GenerateSeats([FromBody] SeatGenerateRequest request)
        {
            if (request.NumberOfRows <= 0 || request.SeatsPerRow <= 0)
            {
                return BadRequest("Rows and Seats must be greater than zero.");
            }

            var screen = _context.ScreenDetails
                                 .FirstOrDefault(s => s.ScreenName == request.ScreenName);

            if (screen == null)
            {
                return BadRequest("Screen not found.");
            }

            List<SeatDetails> seats = new List<SeatDetails>();

            for (int row = 0; row < request.NumberOfRows; row++)
            {   
                char rowLetter = (char)('A' + row);

                for (int seat = 1; seat <= request.SeatsPerRow; seat++)
                {
                    seats.Add(new SeatDetails
                    {
                        ScreenId = screen.ScreenId,
                        SeatNumber = $"{rowLetter}{seat}",
                        SeatType = "Regular"
                    });
                }
            }

            _context.SeatDetails.AddRange(seats);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Seats Generated Successfully",
                Screen = screen.ScreenName,
                TotalSeats = seats.Count
            });
        }
        [HttpPut("UpdateSeatType")]
        public IActionResult UpdateSeatType([FromBody] UpdateSeatTypeRequest request)
        {
            var screen = _context.ScreenDetails
                .FirstOrDefault(s => s.ScreenName == request.ScreenName);

            if (screen == null)
            {
                return BadRequest("Screen not found.");
            }

            var seats = _context.SeatDetails
                .Where(s => s.ScreenId == screen.ScreenId &&
                            s.SeatNumber.StartsWith(request.RowName.ToUpper()))
                .ToList();

            if (!seats.Any())
            {
                return BadRequest("No seats found for this row.");
            }

            foreach (var seat in seats)
            {
                seat.SeatType = request.SeatType;
            }

            _context.SaveChanges();

            return Ok(new
            {
                Message = "Seat type updated successfully.",
                Screen = request.ScreenName,
                Row = request.RowName.ToUpper(),
                UpdatedSeats = seats.Count
            });
        }
    }
}