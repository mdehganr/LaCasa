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
}
