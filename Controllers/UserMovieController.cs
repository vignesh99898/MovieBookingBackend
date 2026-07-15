using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserMovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserMovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetActiveMovies")]
        public IActionResult GetActiveMovies()
        {
            try
            {
                var activeMovies = _context.MovieDetails
                    .Where(m => m.Status == "Active")
                    .Select(m => new
                    {
                        m.MovieName,
                        m.Language,
                        m.Duration,
                        m.ReleaseDate
                    })
                    .ToList();

                if (!activeMovies.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "No active movies found."
                    });
                }

                return Ok(new
                {
                    Status = "Success",
                    Message = "Active movies retrieved successfully.",
                    Data = activeMovies
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