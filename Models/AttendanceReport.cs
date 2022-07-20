using System.ComponentModel.DataAnnotations;

namespace EmployeeAttendanceSystem.Models
{
    public class AttendanceReport
    {
        public int AttendanceId { get; set; }
        public int EmployeeID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
        public DateTime Date { get; set; }
        public Status Status { get; set; }


    }
}
