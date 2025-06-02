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
        public async Task<bool> CanBookAsync(Booking booking)
        {
            var existing = await _context.Bookings
                .Where(b => b.StartDate == DateTime.Now)
                .FirstOrDefaultAsync();

            if (existing != null)
                return false;

            var duration = (booking.EndDate - booking.StartDate).TotalDays;
            return duration > 0 && duration <= 2;
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            if (!await CanBookAsync(booking))
                return false;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            await _webSocketManager.BroadcastBookingEventAsync(new BookingEventDto
            {
                Type = "CREATE", // or "UPDATE"/"DELETE"
                Booking = booking
            });
            return true;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
{
    return await _context.Bookings.ToListAsync();
}
    }
}
