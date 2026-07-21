using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Customer;
using MovieBookingBackend.Data;
using MovieBookingBackend.Services;
using Microsoft.AspNetCore.Identity;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<CustomerDetails> _passwordHasher;

        public CustomerController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<CustomerDetails>();
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
                PhoneNumber = request.PhoneNumber,
                Role = "User"
            };

            // Hash the password
            customer.Password = _passwordHasher.HashPassword(customer, request.Password);

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
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] CustomerLogin request)
        {
            try
            {
                // Check if request is null
                if (request == null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Invalid Request."
                    });
                }

                // Check whether email and password are provided
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Email and Password are required."
                    });
                }

                // Find customer by email
                var customer = _context.CustomerDetails
                                       .FirstOrDefault(x => x.Email == request.Email);

                // Customer not found
                if (customer == null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Invalid Email."
                    });
                }

                // Verify the hashed password
                var result = _passwordHasher.VerifyHashedPassword(
                    customer,
                    customer.Password,
                    request.Password
                );

                if (result == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Invalid Password."
                    });
                }
                // Generate JWT Token
                string token = _jwtService.GenerateToken(
                    customer.UserId,
                    customer.Email,
                    customer.Role
                );

                // Return success response
                return Ok(new
                {
                    Status = "Success",
                    Message = "Login Successful.",
                    Token = token,
                    User = new
                    {
                        customer.UserId,
                        customer.FullName,
                        customer.Email,
                        customer.PhoneNumber,
                        customer.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Login Failed.",
                    Error = ex.Message
                });
            }
        }
    }
}