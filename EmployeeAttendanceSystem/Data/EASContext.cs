using EmployeeAttendanceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceSystem.Data
{
    public class EASContext : DbContext
    {
        public EASContext(DbContextOptions<EASContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Attendance>().ToTable("Attendance");
        }
    }
}