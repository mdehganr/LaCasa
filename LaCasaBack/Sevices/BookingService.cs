using System;
using System.Linq;
using System.Threading.Tasks;
using LaCasa.Data;
using LaCasa.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaCasaBack.Sevices;
using Microsoft.AspNetCore.Http;

namespace LaCasa.Services
{
    public class BookingService 
    {
        private readonly AppDbContext _context;
        private readonly IWebSocketManager _webSocketManager;
        public BookingService(AppDbContext context, IWebSocketManager webSocketManager)
        {
            _context = context;
            _webSocketManager = webSocketManager;
        }   

        public async Task<BookingValidationResult> CanBookAsync(Booking booking)
        {
            var existing = await _context.Bookings
                .Where(b => b.StartDate == booking.StartDate && b.EndDate == booking.EndDate)
                .FirstOrDefaultAsync();

            //Here is the waitlist
            if (booking.Waitlist == false) { 

            if (existing != null)
                return new BookingValidationResult(false, "These dates are already booked");
            }
            else
            {
                return new BookingValidationResult(true, "Your booking has been placed on the waitlist. Please wait for final confirmation.");
            }


                var duration = (booking.EndDate - booking.StartDate).TotalDays;
            
            if (duration <= 0)
                return new BookingValidationResult(false, "End date must be after start date");
                
            if (duration > 2)
                return new BookingValidationResult(false, "Booking duration cannot exceed 2 days");

            return new BookingValidationResult(true, "Booking is available");
        }

        public async Task<BookingValidationResult> CreateBookingAsync(Booking booking)
        {
            var validationResult = await CanBookAsync(booking);
            if (!validationResult.IsValid)
                return validationResult;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            await _webSocketManager.BroadcastBookingEventAsync(new BookingEventDto
            {
                Type = "CREATE",
                Booking = booking
            });
            return new BookingValidationResult(true, "Booking created successfully");
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
    }
}
