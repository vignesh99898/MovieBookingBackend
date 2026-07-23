using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Feedback;
using System.Security.Claims;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "User")]
        [HttpPost("AddFeedback")]
        public IActionResult AddFeedback([FromBody] FeedbackAdd request)
        {
            try
            {
                // Check Request
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Step 1 : Get Logged-in User
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        Status = "Error",
                        Message = "Invalid Token."
                    });
                }

                int userId = Convert.ToInt32(userIdClaim.Value);

                // Step 2 : Find Customer
                var customer = _context.CustomerDetails
                    .FirstOrDefault(c => c.UserId == userId);

                if (customer == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "User not found."
                    });
                }

                // Step 3 : Find Movie
                var movie = _context.MovieDetails
                    .FirstOrDefault(m =>
                        m.MovieName == request.MovieName &&
                        m.Status == "Active");

                if (movie == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie not found."
                    });
                }

                // Step 4 : Find Movie Schedule
                var schedule = _context.MovieScheduleDetails
                    .FirstOrDefault(ms =>
                        ms.MovieId == movie.MovieId &&
                        ms.Status == "Active");

                if (schedule == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie schedule not found."
                    });
                }

                // Step 5 : Find Show
                var show = _context.ShowTimeDetails
                    .FirstOrDefault(st =>
                        st.ScheduleId == schedule.ScheduleId &&
                        st.ShowDate.Date == request.ShowDate.Date &&
                        st.StartTime == request.ShowTime);

                if (show == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Show not found."
                    });
                }

                // Step 6 : Find Booking
                var booking = _context.BookingDetails
                    .FirstOrDefault(b =>
                        b.UserId == userId &&
                        b.ShowId == show.ShowId &&
                        b.BookingStatus == "Booked");

                if (booking == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Booking not found."
                    });
                }

                // Step 7 : Check Show End Time
                DateTime showEndDateTime =
                    show.ShowDate.Date + show.EndTime;

                if (DateTime.Now < showEndDateTime)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Feedback can only be submitted after the show has ended."
                    });
                }

                // Step 8 : Check Existing Feedback
                var existingFeedback = _context.FeedbackDetails
                    .FirstOrDefault(f => f.BookingId == booking.BookingId);

                if (existingFeedback != null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Feedback has already been submitted."
                    });
                }

                // Step 9 : Save Feedback
                FeedbackDetails feedback = new FeedbackDetails
                {
                    BookingId = booking.BookingId,
                    UserId = userId,
                    MovieId = movie.MovieId,
                    Rating = request.Rating,
                    Review = request.Review,
                    FeedbackDate = DateTime.Now
                };

                _context.FeedbackDetails.Add(feedback);
                _context.SaveChanges();

                // Step 10 : Return Success
                return Ok(new
                {
                    Status = "Success",
                    Message = "Feedback submitted successfully.",
                    Feedback = new
                    {
                        customer.FullName,
                        Movie = movie.MovieName,
                        Rating = feedback.Rating,
                        Review = feedback.Review,
                        SubmittedOn = feedback.FeedbackDate
                    }
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