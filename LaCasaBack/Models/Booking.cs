using System;
using System.ComponentModel.DataAnnotations;

namespace LaCasa.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Guest { get; set; }
     
        [Required]
        public string EmployeeEmail { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
    public class BookingEventDto
    {
        public string Type { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public Booking Booking { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
