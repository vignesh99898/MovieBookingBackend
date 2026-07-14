using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Data;
using MovieBookingBackend.Theatre;

namespace MovieBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheatreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TheatreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddTheatre")]
        public IActionResult AddTheatre([FromBody] TheatreAdd request)
        {
            // Check if theatre already exists
            var existingTheatre = _context.TheatreDetails
                                          .FirstOrDefault(x => x.TheatreName == request.TheatreName);

            if (existingTheatre != null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Theatre already exists."
                });
            }

            TheatreDetails theatre = new TheatreDetails
            {
                TheatreName = request.TheatreName,
                Location = request.Location,
                Address = request.Address
            };

            try
            {
                _context.TheatreDetails.Add(theatre);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Theatre added successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Unable to add theatre.",
                    Error = ex.Message
                });
            }
        }
        [HttpPut("InactiveTheatre")]
        public IActionResult InactiveTheatre(string theatreName)
        {
            var theatre = _context.TheatreDetails
                                  .FirstOrDefault(t => t.TheatreName == theatreName);

            if (theatre == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            // Check if the theatre is not Active
            if (theatre.Status != "Active")
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Only active theatres can be changed to inactive."
                });
            }

            theatre.Status = "Inactive";

            try
            {
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Theatre status updated to Inactive successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                            Status = "Error",
                            Message = "Unable to update theatre status.",
                            Error = ex.Message
                });
            }
        }
        [HttpPut("ActiveTheatre")]
        public IActionResult ActiveTheatre(string theatreName)
        {
            var theatre = _context.TheatreDetails
                                  .FirstOrDefault(t => t.TheatreName == theatreName);

            if (theatre == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            if (theatre.Status != "Inactive")
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Theatre is already active."
                });
            }

            theatre.Status = "Active";

            _context.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "Theatre status updated to Active successfully."
            });
        }
        [HttpDelete("DeleteTheatre")]
        public IActionResult DeleteTheatre(string theatreName)
        {
            // Check if the theatre exists
            var theatre = _context.TheatreDetails
                                  .FirstOrDefault(t => t.TheatreName == theatreName);

            if (theatre == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Theatre not found."
                });
            }

            try
            {   
                _context.TheatreDetails.Remove(theatre);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Theatre deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Unable to delete theatre.",
                    Error = ex.Message
                });
            }
        }
        [HttpGet("GetInactiveTheatres")]
        public IActionResult GetInactiveTheatres()
        {
            var theatres = _context.TheatreDetails
                                   .Where(t => t.Status == "Inactive")
                                   .Select(t => new
                                   {
                                       t.TheatreId,
                                       t.TheatreName,
                                       t.Location,
                                       t.Address,
                                       t.Status
                                   })
                                   .ToList();

            if (!theatres.Any())
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "No inactive theatres found."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Inactive theatres retrieved successfully.",
                Data = theatres
            });
        }
        [HttpGet("GetActiveTheatres")]
        public IActionResult GetActiveTheatres()
        {
            var theatres = _context.TheatreDetails
                                   .Where(t => t.Status == "Active")
                                   .Select(t => new
                                   {
                                       t.TheatreId,
                                       t.TheatreName,
                                       t.Location,
                                       t.Address,
                                       t.Status
                                   })
                                   .ToList();

            if (!theatres.Any())
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "No active theatres found."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Active theatres retrieved successfully.",
                Data = theatres
            });
        }       
    }
}