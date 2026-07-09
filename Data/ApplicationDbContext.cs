using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Customer;
using MovieBookingBackend.Movie;

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
    }
}