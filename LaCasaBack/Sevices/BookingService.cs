using System;
using System.Linq;
using System.Threading.Tasks;
using LaCasa.Data;
using LaCasa.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaCasa.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
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
            return true;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
{
    return await _context.Bookings.ToListAsync();
}
    }
}
