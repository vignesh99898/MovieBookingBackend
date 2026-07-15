using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTheatreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserTheatreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetTheatresByMovie")]
        public IActionResult GetTheatresByMovie(string movieName)
        {
            try
            {
                // Find Movie
                var movie = _context.MovieDetails
                    .FirstOrDefault(m => m.MovieName == movieName && m.Status == "Active");

                if (movie == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie not found."
                    });
                }

                // Get Theatre Ids from MovieSchedule
                var theatreIds = _context.MovieScheduleDetails
                    .Where(ms => ms.MovieId == movie.MovieId)
                    .Select(ms => ms.TheatreId)
                    .Distinct()
                    .ToList();

                if (!theatreIds.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "No theatres found for this movie."
                    });
                }

                // Get Theatre Details
                var theatres = _context.TheatreDetails
                    .Where(t => theatreIds.Contains(t.TheatreId) && t.Status == "Active")
                    .Select(t => new
                    {
                        t.TheatreName
                    })
                    .ToList();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Theatres retrieved successfully.",
                    Data = theatres
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