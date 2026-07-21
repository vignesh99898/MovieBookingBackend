using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieBookingBackend.Data;
using MovieBookingBackend.Movie;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.Screen;
using MovieBookingBackend.ShowTime;
using MovieBookingBackend.Theatre;
using MovieBookingBackend.Booking;
using MovieBookingBackend.Seat;

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

        [AllowAnonymous]
        [HttpGet("GetAvailableSeats")]
        public IActionResult GetAvailableSeats(
            string movieName,
            string theatreName,
            DateTime showDate,
            TimeSpan startTime)
        {
            try
            {
                //Find Movie
                var movie = _context.MovieDetails
                    .FirstOrDefault(m =>
                        m.MovieName == movieName &&
                        m.Status == "Active");

                if (movie == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie not found."
                    });
                }

                //Find Theatre
                var theatre = _context.TheatreDetails
                    .FirstOrDefault(t =>
                        t.TheatreName == theatreName &&
                        t.Status == "Active");

                if (theatre == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Theatre not found."
                    });
                }

                // Find Schedule
                var schedule = _context.MovieScheduleDetails
                    .FirstOrDefault(ms =>
                        ms.MovieId == movie.MovieId &&
                        ms.TheatreId == theatre.TheatreId &&
                        ms.Status == "Active");

                if (schedule == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie schedule not found."
                    });
                }

                //Find Show
                var show = _context.ShowTimeDetails
                    .FirstOrDefault(st =>
                        st.ScheduleId == schedule.ScheduleId &&
                        st.ShowDate.Date == showDate.Date &&
                        st.StartTime == startTime);

                if (show == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Show not found."
                    });
                }

                //Get all seats of the screen
                var allSeats = _context.SeatDetails
                    .Where(s => s.ScreenId == show.ScreenId)
                    .ToList();

                //Get BookingIds
                var bookingIds = _context.BookingDetails
                    .Where(b =>
                        b.ShowId == show.ShowId &&
                        b.BookingStatus == "Booked")
                    .Select(b => b.BookingId)
                    .ToList();

                //Get booked SeatIds
                var bookedSeatIds = _context.BookingSeatDetails
                    .Where(bs => bookingIds.Contains(bs.BookingId))
                    .Select(bs => bs.SeatId)
                    .ToList();

                //Return available seats
                var availableSeats = allSeats
                    .Where(s => !bookedSeatIds.Contains(s.SeatId))
                    .Select(s => new
                    {
                        s.SeatId,
                        s.SeatNumber,
                        s.SeatType
                    })
                    .OrderBy(s => s.SeatNumber)
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