using Microsoft.AspNetCore.Mvc;
using LaCasa.Models;
using LaCasa.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace LaCasa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        public  BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] Booking booking)
        {
            var validationResult = await _bookingService.CreateBookingAsync(booking);
            if (validationResult.IsValid)
                return Ok(booking);

            return BadRequest(validationResult.Message);
        }

        // GET api/booking
        [HttpGet]
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateStatusRequest req)
        {
         
            var result = await _bookingService.UpdateBookingStatusAsync(id, req.Status);
            if (req.Status == BookingStatus.Canceled)
            {
                _bookingService.WaitlistReplacement(id);
            }
            return result.IsValid ? Ok() : BadRequest(result.Message);
        }

        public record UpdateStatusRequest(BookingStatus Status);

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByStatus(BookingStatus status)
        {
      
            var bookings = await _bookingService.GetBookingsByStatusAsync(status);
            return Ok(bookings);
        }
    }
}
