using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Customer;
using MovieBookingBackend.Movie;
using MovieBookingBackend.Theatre;
using MovieBookingBackend.Screen;
using MovieBookingBackend.Seat;
using MovieBookingBackend.MovieSchedule;

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
    }
}