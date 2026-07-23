using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Customer;
using MovieBookingBackend.Movie;
using MovieBookingBackend.Theatre;
using MovieBookingBackend.Screen;
using MovieBookingBackend.Seat;
using MovieBookingBackend.MovieSchedule;
using MovieBookingBackend.ShowTime;
using MovieBookingBackend.Booking;

namespace MovieBookingBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MovieDetails> MovieDetails { get; set; }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }
        public DbSet<TheatreDetails> TheatreDetails { get; set; }
        public DbSet<ScreenDetails> ScreenDetails { get; set; }
        public DbSet<SeatDetails> SeatDetails { get; set; }
        public DbSet<MovieScheduleDetails> MovieScheduleDetails { get; set; }
        public DbSet<ShowTime.ShowTimeDetails> ShowTimeDetails { get; set; }
        public DbSet<Booking.BookingDetails> BookingDetails { get; set; }
        public DbSet<Booking.BookingSeatDetails> BookingSeatDetails { get; set; }
        public DbSet<Feedback.FeedbackDetails> FeedbackDetails { get; set; }
    }
}