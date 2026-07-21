using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieBookingBackend.Data;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserShowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserShowController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("GetShowTimes")]
        public IActionResult GetShowTimes(string movieName, string theatreName)
        {
            try
            {
                // Find Movie
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

                // Find Theatre
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

                // Find Movie Schedule
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

                // Get Show Times
                var showTimes = _context.ShowTimeDetails
                    .Where(st => st.ScheduleId == schedule.ScheduleId)
                    .Select(st => new
                    {
                        st.ShowId,
                        st.ShowDate,
                        st.StartTime,
                        st.EndTime,
                        st.AvailableSeats
                    })
                    .ToList();

                if (!showTimes.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "No show times available."
                    });
                }

                return Ok(new
                {
                    Status = "Success",
                    Message = "Show times retrieved successfully.",
                    Data = showTimes
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