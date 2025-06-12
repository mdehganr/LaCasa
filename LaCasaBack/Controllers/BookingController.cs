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
            var validationResult = await _bookingService.CreateBookingAsync(booking);
            if (validationResult.IsValid) // Assuming BookingValidationResult has an IsSuccess property
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
    }
}
