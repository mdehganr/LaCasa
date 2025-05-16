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
            if (await _bookingService.CreateBookingAsync(booking))
                return Ok("Booking successful.");

            return BadRequest("Booking failed. Ensure no existing booking this year and max 2 days.");
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
