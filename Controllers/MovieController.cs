using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Movie;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddMovie"    )]
        public IActionResult AddMovie(string adminEmail, [FromBody] MovieAdd request)
        {
            // Check whether the user is an Admin
            var admin = _context.CustomerDetails
                                .FirstOrDefault(x => x.Email == adminEmail);

            if (admin == null || admin.Role != "Admin")
            {
                return Unauthorized(new
                {
                    Status = "Error",
                    Message = "Only Admin can add movies."
                });
            }

            // Check if the movie already exists
            var existingMovie = _context.MovieDetails
                                .FirstOrDefault(x => x.MovieName == request.MovieName);

            if (existingMovie != null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Movie already exists."
                });
            }

            // Create a new movie object
            MovieDetails movie = new MovieDetails
            {
                MovieName = request.MovieName,
                Language = request.Language,
                Duration = request.Duration,
                ReleaseDate = request.ReleaseDate,
            };

            try
            {
                _context.MovieDetails.Add(movie);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Movie added successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Unable to add the movie.",
                    Error = ex.Message
                });
            }
        
        }
        [HttpDelete("DeleteMovie")]
        public IActionResult DeleteMovie(string adminEmail, string movieName)
        {
            var admin = _context.CustomerDetails
                .FirstOrDefault(x => x.Email == adminEmail);

            if (admin == null || admin.Role != "Admin")
            {
                return Unauthorized(new
                {
                    Status = "Error",
                    Message = "Only Admin can delete movies."
                });
            }

            var movie = _context.MovieDetails
                .FirstOrDefault(x => x.MovieName == movieName);

            if (movie == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Movie not found."
                });
            }

            _context.MovieDetails.Remove(movie);
            _context.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "Movie deleted successfully."
            });
        }
    }
}