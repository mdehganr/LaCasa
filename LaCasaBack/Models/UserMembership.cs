using System.ComponentModel.DataAnnotations;

namespace LaCasa.Models
{
    public enum MembershipType
    {
        Regular = 0,
        SLT = 1,
        ELT = 2
    }

    public class UserMembership
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EmployeeEmail { get; set; }
        [Required]
        public MembershipType MembershipType { get; set; }
    }
}