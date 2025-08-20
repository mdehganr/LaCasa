using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LaCasa.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookingStatus
    {
        Submitted,
        Confirmed,
        Waitlist,
        Canceled
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
     
        [Required]
        public string EmployeeEmail { get; set; }
        [Required]
        public string Guest { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset EndDate { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Submitted;
    }

    public class BookingEventDto
    {
        public string Type { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public Booking Booking { get; set; } = new();
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
    }
}
