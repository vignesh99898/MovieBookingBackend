using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Customer;
using MovieBookingBackend.Data;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] CustomerRegister request)
        {
            // Check whether Password and Confirm Password are the same
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Password and Confirm Password do not match."
                });
            }

            // Check whether Email already exists
            var existingUser = _context.CustomerDetails
                                       .FirstOrDefault(x => x.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Email already exists."
                });
            }

            // Create a new customer object
            CustomerDetails customer = new CustomerDetails
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber
            };

            try
            {
                _context.CustomerDetails.Add(customer);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Customer registered successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Registration failed.",
                    Error = ex.Message
                });
            }
        }
        

   
        [HttpPost("Login")]
        public IActionResult Login([FromBody] CustomerLogin request)
        {
       // Search for a customer with the given email and password
            var customer = _context.CustomerDetails
                                   .FirstOrDefault(x =>
                                       x.Email == request.Email &&
                                       x.Password == request.Password);

            // Customer not found
            if (customer == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Invalid Email or Password."
                });
            }

            // Customer found
            return Ok(new
            {
                Status = "Success",
                Message = "Login Successful.",
                User = new
                {
                    customer.UserId,
                    customer.FullName,
                    customer.Email,
                    customer.PhoneNumber
                }
            });
        }
    }
}