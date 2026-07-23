using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieBookingBackend.Data;
using MovieBookingBackend.Booking;
using MovieBookingBackend.Movie;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.ShowTime;
using MovieBookingBackend.Theatre;
using MovieBookingBackend.Seat;
using MovieBookingBackend.Services;
using MovieBookingBackend.Customer;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public BookingController(
            ApplicationDbContext context,
            EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        // Book Tickets
        [AllowAnonymous]
        [HttpPost("BookTickets")]
        public IActionResult BookTickets([FromBody] BookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Find Movie
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

                //Find Theatre
                var theatre = _context.TheatreDetails
                    .FirstOrDefault(t =>
                        t.TheatreName == request.TheatreName &&
                        t.Status == "Active");

                if (theatre == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Theatre not found."
                    });
                }
                //Find Movie Schedule
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
                        st.ShowDate.Date == request.ShowDate.Date &&
                        st.StartTime == request.StartTime);

                if (show == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Show not found."
                    });
                }
                //Check Show Time

                DateTime showStartDateTime = show.ShowDate.Date + show.StartTime;

                if (DateTime.Now >= showStartDateTime)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Booking is closed. The show has already started."
                    });
                }
                //Find SeatIds
                var selectedSeats = new List<SeatDetails>();

                foreach (var seat in request.Seats)
                {
                    var seatDetails = _context.SeatDetails
                        .FirstOrDefault(s =>
                            s.ScreenId == show.ScreenId &&
                            s.SeatNumber == seat.SeatNumber);

                    if (seatDetails == null)
                    {
                        return NotFound(new
                
                        {
                            Status = "Error",
                            Message = $"Seat '{seat.SeatNumber}' not found."
                        });
                    }

                    selectedSeats.Add(seatDetails);
                }
                //Check Seat Availability

                foreach (var seat in selectedSeats)
                    {
                bool isBooked = _context.BookingSeatDetails
                    .Join(_context.BookingDetails,
                        bookingSeat => bookingSeat.BookingId,
                        booking => booking.BookingId,
                        (bookingSeat, booking) => new
                        {
                            bookingSeat.SeatId,
                            booking.ShowId,
                            booking.BookingStatus
                        })
                        .Any(x =>
                            x.SeatId == seat.SeatId &&
                            x.ShowId == show.ShowId &&
                            x.BookingStatus == "Booked");

                    if (isBooked)
                    {
                        return BadRequest(new
                        {
                            Status = "Error",
                            Message = $"Seat {seat.SeatNumber} is already booked."
                        });
                    }
                }
                //Calculate Ticket Amount

                decimal totalAmount = 0;

                foreach (var seat in selectedSeats)
                {
                    decimal seatPrice = movie.TicketPrice;

                    if (seat.SeatType.Equals("Premium", StringComparison.OrdinalIgnoreCase))
                    {
                        seatPrice += 50;
                    }

                    totalAmount += seatPrice;
                }
                //Save BookingDetails

                var booking = new BookingDetails
                {
                    UserId = request.UserId,
                    ShowId = show.ShowId,
                    BookingDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    BookingStatus = "Booked"
                };

                _context.BookingDetails.Add(booking);
                _context.SaveChanges();
                //Save BookingSeatDetails

                foreach (var seat in selectedSeats)
                {
                    BookingSeatDetails bookingSeat = new BookingSeatDetails
                    {
                        BookingId = booking.BookingId,
                        SeatId = seat.SeatId
                    };

                    _context.BookingSeatDetails.Add(bookingSeat);
                }

                _context.SaveChanges();

                // Find Customer
                var customer = _context.CustomerDetails
                    .FirstOrDefault(c => c.UserId == booking.UserId);

                if (customer == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Customer not found."
                    });
                }

                // Prepare Seat Numbers
                List<string> seatNumbers = selectedSeats
                    .Select(s => s.SeatNumber)
                    .ToList();

                // Send Email
                _emailService.SendBookingConfirmation(
                    customer.Email,
                    customer.FullName,
                    movie.MovieName,
                    theatre.TheatreName,
                    show.ShowDate,
                    show.StartTime,
                    seatNumbers,
                    totalAmount,
                    booking.BookingId
                );

                // Return Success
                return Ok(new
                {
                    Status = "Success",
                    Message = "Booking Successful. Confirmation Email Sent.",
                    BookingId = booking.BookingId,
                    TotalAmount = totalAmount
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

        // Booking History
        [Authorize(Roles = "User")]
        [HttpGet("GetBookingHistory")]
        public IActionResult GetBookingHistory(int userId)
        {
            try
            {
                var bookings = _context.BookingDetails
                    .Where(b => b.UserId == userId)
                    .ToList();
                if (!bookings.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "No bookings found."
                    });
                }
                return Ok(new
                {
                    Status = "Success",
                    Message = "Booking history retrieved successfully.",
                    Data = bookings
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
        // Booking Details
        [AllowAnonymous]
        [HttpGet("GetBookingDetails")]
        public IActionResult GetBookingDetails(int bookingId)
        {
            try
            {
                // Step 1 : Find Booking
                var booking = _context.BookingDetails
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Booking not found."
                    });
                }

                // Step 2 : Find Show
                var show = _context.ShowTimeDetails
                    .FirstOrDefault(s => s.ShowId == booking.ShowId);

                if (show == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Show not found."
                    });
                }

                // Step 3 : Find Schedule
                var schedule = _context.MovieScheduleDetails
                    .FirstOrDefault(ms => ms.ScheduleId == show.ScheduleId);

                if (schedule == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Movie schedule not found."
                    });
                }

                // Step 4 : Find Movie
                var movie = _context.MovieDetails
                    .FirstOrDefault(m => m.MovieId == schedule.MovieId);

                // Step 5 : Find Theatre
                var theatre = _context.TheatreDetails
                    .FirstOrDefault(t => t.TheatreId == schedule.TheatreId);

                // Step 6 : Find Seats
                var seatIds = _context.BookingSeatDetails
                    .Where(bs => bs.BookingId == booking.BookingId)
                    .Select(bs => bs.SeatId)
                    .ToList();

                var seats = _context.SeatDetails
                    .Where(s => seatIds.Contains(s.SeatId))
                    .Select(s => s.SeatNumber)
                    .ToList();

                // Step 7 : Return Response
                return Ok(new
                {
                    Status = "Success",
                    BookingId = booking.BookingId,
                    MovieName = movie?.MovieName,
                    TheatreName = theatre?.TheatreName,
                    ShowDate = show.ShowDate,
                    ShowTime = show.StartTime,
                    Seats = seats,
                    TotalAmount = booking.TotalAmount,
                    BookingStatus = booking.BookingStatus,
                    BookingDate = booking.BookingDate
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
        // Cancel Booking
        [AllowAnonymous]
        [HttpPut("CancelTicket")]
        public IActionResult CancelTicket(int bookingId)
        {
            try
            {
                // Step 1 : Find Booking
                var booking = _context.BookingDetails
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Booking not found."
                    });
                }

                // Step 2 : Check Already Cancelled
                if (booking.BookingStatus == "Cancelled")
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Ticket is already cancelled."
                    });
                }

                // Step 3 : Update Booking Status
                booking.BookingStatus = "Cancelled";

                _context.SaveChanges();

                // Step 4 : Return Success
                return Ok(new
                {
                    Status = "Success",
                    Message = "Ticket cancelled successfully."
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