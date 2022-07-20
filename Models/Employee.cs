namespace EmployeeAttendanceSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        public string? Role { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }
        [DisplayFormat(NullDisplayText = "")]
        public double Phone { get; set; }
        [Required]
        public string? Gender { get; set; }

    }
}