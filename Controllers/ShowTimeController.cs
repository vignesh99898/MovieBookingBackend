using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Movie;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.Screen;
using MovieBookingBackend.ShowTime;
using MovieBookingBackend.Theatre;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowTimeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowTimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddShowTime")]
        public IActionResult AddShowTime([FromBody] ShowTimeRequest request)
        {

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

            if (theatre.Status != "Active")
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Theatre is Inactive."
                });
            }

            var screen = _context.ScreenDetails
                .FirstOrDefault(s =>
                    s.ScreenName == request.ScreenName &&
                    s.TheatreId == theatre.TheatreId);

            if (screen == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Screen not found in selected theatre."
                });
            }

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
                    Message = "Movie Schedule not found."
                });
            }

            int totalSeats = screen.TotalSeats;

            List<ShowTimeDetails> showList = new List<ShowTimeDetails>();

            DateTime currentDate = schedule.StartDate;

            while (currentDate <= schedule.EndDate)
            {
                foreach (var timing in request.ShowTimings)
                {
                    // Check whether the same show already exists
                    bool showExists = _context.ShowTimeDetails.Any(st =>
                        st.ScheduleId == schedule.ScheduleId &&
                        st.ScreenId == screen.ScreenId &&
                        st.ShowDate == currentDate &&
                        st.StartTime == timing.StartTime);

                    if (showExists)
                    {
                        continue;
                    }

                    ShowTimeDetails show = new ShowTimeDetails
                    {
                        ScheduleId = schedule.ScheduleId,

                        ScreenId = screen.ScreenId,

                        ShowDate = currentDate,

                        StartTime = timing.StartTime,

                        EndTime = timing.EndTime,

                        AvailableSeats = totalSeats
                    };

                    showList.Add(show);
                }

                currentDate = currentDate.AddDays(1);
            }

            if (showList.Count == 0)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "All show timings already exist."
                });
            }

            _context.ShowTimeDetails.AddRange(showList);

            _context.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "Show timings created successfully.",
                TotalShowsCreated = showList.Count
            });
        }
        [HttpGet("GetAvailableShows")]
        public IActionResult GetAvailableShows()
        {
            DateTime today = DateTime.Today;
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            var shows = _context.ShowTimeDetails
                .Where(s =>
                    s.ShowDate.Date > today.Date ||
                    (s.ShowDate.Date == today.Date &&
                     s.StartTime >= currentTime))
                .Select(s => new
                {
                    s.ShowId,
                    s.ScheduleId,
                    s.ScreenId,
                    s.ShowDate,
                    s.StartTime,
                    s.EndTime,
                    s.AvailableSeats
                })
                .ToList();

            if (!shows.Any())
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "No upcoming shows available."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Data = shows
            });
        }
    }
} 