using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieBookingBackend.Data;
using MovieBookingBackend.Screen;
using MovieBookingBackend.Theatre;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScreenController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddScreen")]
        public IActionResult AddScreen([FromBody] ScreenAdd request)
        {
            // Check whether the theatre exists
            var theatre = _context.TheatreDetails
                                  .FirstOrDefault(t => t.TheatreName == request.TheatreName);

            if (theatre == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            // Check if the screen already exists in the same theatre
            var existingScreen = _context.ScreenDetails
                                         .FirstOrDefault(s =>
                                             s.TheatreId == theatre.TheatreId &&
                                             s.ScreenName == request.ScreenName);

            if (existingScreen != null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Screen already exists in this theatre."
                });
            }

            // Create Screen
            ScreenDetails screen = new ScreenDetails
            {
                TheatreId = theatre.TheatreId,
                ScreenName = request.ScreenName,
                TotalSeats = request.TotalSeats
            };

            try
            {
                _context.ScreenDetails.Add(screen);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Screen added successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Unable to add screen.",
                    Error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetScreensByTheatre")]
        public IActionResult GetScreensByTheatre(string theatreName)
        {
            // Check whether the theatre exists
            var theatre = _context.TheatreDetails
                                  .FirstOrDefault(t => t.TheatreName == theatreName);

            if (theatre == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            // Get all screens for the theatre
            var screens = _context.ScreenDetails
                                  .Where(s => s.TheatreId == theatre.TheatreId)
                                  .Select(s => new
                                  {
                                      s.ScreenId,
                                      s.ScreenName,
                                      s.TotalSeats
                                  })
                                  .ToList();

            if (!screens.Any())
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "No screens found for this theatre."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Screens retrieved successfully.",
                Data = screens
            });
        }
    }
}