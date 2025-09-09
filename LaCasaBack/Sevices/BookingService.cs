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
using LaCasa.Controllers;

namespace LaCasa.Services
{
    public class BookingService 
    {
        public readonly AppDbContext _context;
        public readonly IWebSocketManager _webSocketManager;
        public BookingService(AppDbContext context, IWebSocketManager webSocketManager)
        {
            _context = context;
            _webSocketManager = webSocketManager;
        }   



        public async Task<BookingValidationResult> CreateBookingAsync(Booking booking)
        {
            var validStay = await IsStayPolicyValid(booking);
            if (!validStay.IsValid)
                return validStay;
            if (booking.Status!=BookingStatus.Waitlisted)
            {

            var validDates=await ValidateBookingAsync(booking);
            if (!validDates.IsValid)
              return validDates;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            await _webSocketManager.BroadcastBookingEventAsync(new BookingEventDto
            {
                Type = "CREATE",
                Booking = booking
            });
            return new BookingValidationResult(true, "Booking created successfully");
        }

        public async Task<BookingValidationResult> ValidateBookingAsync(Booking booking)
        {
            // Normalize & basic guardrails
            var start = booking.StartDate;
            var end = booking.EndDate;

            if (end <= start)
                return new BookingValidationResult(false, "End date must be after start date.");

            // Any overlap with an active booking for the same room?
            var overlaps = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Canceled)
                // Half-open interval overlap check:
                .Where(b => b.StartDate < end && start < b.EndDate && b.Id!=booking.Id)
                .AnyAsync();

            if (overlaps && booking.Status!=BookingStatus.Waitlisted)
            {
                booking.Status = BookingStatus.Waitlisted;
                return new BookingValidationResult(false,
                    "Requested dates overlap an existing booking.");
            }

            // Available
            return new BookingValidationResult(true, "Booking is available.");
        }

        public Task<BookingValidationResult> IsStayPolicyValid(Booking booking)
        {
            var duration = (booking.EndDate - booking.StartDate).TotalDays;

            if (duration <= 0)
                return Task.FromResult(new BookingValidationResult(false, "End date must be after start date"));

            if (duration > 2)
                return Task.FromResult(new BookingValidationResult(false, "Booking duration cannot exceed 2 days for employees"));

            return Task.FromResult(new BookingValidationResult(true, "Booking is available"));
        }
        public async Task<BookingValidationResult> UpdateBookingStatusAsync(int id, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return new BookingValidationResult(false, "Booking not found");
            booking.Status = status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            await _webSocketManager.BroadcastBookingEventAsync(new BookingEventDto
            {
                Type = "UPDATE",
                Booking = booking
            });
            return new BookingValidationResult(true, "Booking status updated successfully");
        }
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        internal async Task NotifyPromotionAsync(Booking item)
        {
           
            throw new NotImplementedException();
        }

        public  void WaitlistReplacement(int bookingId)
        {
             CheckAvailibilty(bookingId);
        }

        public async void CheckAvailibilty(int bookingId)
        {
            var canceledBooking = await _context.Bookings.FindAsync(bookingId);
          
            if(canceledBooking!=null)
            {
                //// Any overlap with an active booking 
                //var overlaps = await _context.Bookings
                //// Half-open interval overlap check:
                //.Where(b => b.StartDate < canceledBooking.EndDate && canceledBooking.StartDate < b.EndDate)
                //.IEnumerable<Booking>;

                IEnumerable<Booking> result = _context.Bookings
                  .Where(b => b.StartDate < canceledBooking.EndDate && canceledBooking.StartDate < b.EndDate && b.Status!=BookingStatus.Canceled)
                     .AsEnumerable();
            }
        }
    }
}
