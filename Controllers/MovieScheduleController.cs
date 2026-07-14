using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Movie;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.Screen;
using MovieBookingBackend.Theatre;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddMovieSchedule")]
        public IActionResult AddMovieSchedule([FromBody] MovieScheduleRequest request)
        {
            // Check whether Movie exists
            var movie = _context.MovieDetails
                .FirstOrDefault(m => m.MovieName == request.MovieName);

            if (movie == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Movie not found."
                });
            }

            // Check Start Date and End Date
            if (request.EndDate < request.StartDate)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "End Date cannot be earlier than Start Date."
                });
            }

            // Check Theatre Count
            if (request.NumberOfTheatres != request.TheatreScreenDetails.Count)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Number of theatres does not match the records entered."
                });
            }

            List<MovieScheduleDetails> schedules = new List<MovieScheduleDetails>();

            foreach (var item in request.TheatreScreenDetails)
            {
                // Check Theatre
                var theatre = _context.TheatreDetails
                    .FirstOrDefault(t => t.TheatreName == item.TheatreName);

                if (theatre == null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = $"Theatre '{item.TheatreName}' not found."
                    });
                }

                // Check Theatre Status
                if (theatre.Status != "Active")
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = $"Theatre '{item.TheatreName}' is Inactive."
                    });
                }

                // Find Screen inside that Theatre
                var screen = _context.ScreenDetails
                    .FirstOrDefault(s =>
                        s.ScreenName == item.ScreenName &&
                        s.TheatreId == theatre.TheatreId);

                if (screen == null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = $"Screen '{item.ScreenName}' does not exist in theatre '{item.TheatreName}'."
                    });
                }
                                // Add Schedule Details
                schedules.Add(new MovieScheduleDetails
                {
                    MovieId = movie.MovieId,
                    TheatreId = theatre.TheatreId,
                    ScreenId = screen.ScreenId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = "Active"
                });
            }

            // Save all schedules to the database
            _context.MovieScheduleDetails.AddRange(schedules);
            _context.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "Movie scheduled successfully.",
                TotalSchedules = schedules.Count,
                MovieName = request.MovieName
            });
        }
        [HttpPut("ExtendMovieSchedule")]
        public IActionResult ExtendMovieSchedule([FromBody] ExtendMovieScheduleRequest request)
        {   
            // Find Movie
            var movie = _context.MovieDetails
                .FirstOrDefault(m => m.MovieName == request.MovieName);

            if (movie == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Movie not found."
                });
            }

            // Find Theatre
            var theatre = _context.TheatreDetails
                .FirstOrDefault(t => t.TheatreName == request.TheatreName);

            if (theatre == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            // Find Screen
            var screen = _context.ScreenDetails
                .FirstOrDefault(s =>
                    s.ScreenName == request.ScreenName &&
                    s.TheatreId == theatre.TheatreId);

            if (screen == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Screen not found in the selected theatre."
                });
            }

            // Find Movie Schedule
            var schedule = _context.MovieScheduleDetails
                .FirstOrDefault(ms =>
                    ms.MovieId == movie.MovieId &&
                    ms.TheatreId == theatre.TheatreId &&
                    ms.ScreenId == screen.ScreenId &&
                    ms.Status == "Active");

            if (schedule == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Movie schedule not found."
                });
            }

            // Validate New End Date
            if (request.NewEndDate <= schedule.EndDate)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "New End Date must be greater than the current End Date."
                });
            }

            // Update End Date
            schedule.EndDate = request.NewEndDate;

            _context.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "Movie schedule extended successfully.",
                Movie = request.MovieName,
                Theatre = request.TheatreName,
                Screen = request.ScreenName,
                UpdatedEndDate = schedule.EndDate
            });     
        }
    }
}