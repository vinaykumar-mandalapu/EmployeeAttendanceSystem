using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAttendanceSystem.Models
{
    public enum Status
    {
        A, P
    }
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
    }
}
