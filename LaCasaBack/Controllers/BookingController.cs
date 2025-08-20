using Microsoft.AspNetCore.Mvc;
using LaCasa.Models;
using LaCasa.Services;
using System.Threading.Tasks;

namespace LaCasa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] Booking booking)
        {
            booking.Status = BookingStatus.Submitted; // Ensure new bookings start as Submitted
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
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] BookingStatus status)
        {
            var result = await _bookingService.UpdateBookingStatusAsync(id, status);
            if (result.IsValid)
                return Ok();

            return BadRequest(result.Message);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByStatus(BookingStatus status)
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync(status);
            return Ok(bookings);
        }
    }
}
