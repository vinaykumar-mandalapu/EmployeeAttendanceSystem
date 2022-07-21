using EmployeeAttendanceSystem.Models;
using System;
using System.Linq;

namespace EmployeeAttendanceSystem.Data
{
    public class DbInitializer
    {
        public static void Initialize(EASContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Employees.Any())
            {
                return;   // DB has been seeded
            }

            var employees = new Employee[]
            {
                new Employee{LastName = "Mandalapu", FirstName = "Vinay Kumar", StartDate = DateTime.Now, DOB = DateTime.Parse("1997-09-01"), Gender = "Male", Phone = Convert.ToDouble("3167123016"), Role = "Senior Cloud Architect" },
                new Employee{LastName = "Chamanthula", FirstName = "Ashish", StartDate = DateTime.Now, DOB = DateTime.Parse("1995-04-24"), Gender = "Male", Phone = Convert.ToDouble("3163000234"), Role = "Business Analyst" },
                new Employee{LastName = "Vijjigiri", FirstName = "Sruthibindu", StartDate = DateTime.Now, DOB = DateTime.Parse("1991-08-13"), Gender = "Female", Phone = Convert.ToDouble("6672137479"), Role = "Java Developer" },
                new Employee{LastName = "Podduturi", FirstName = "Rachana Reddy", StartDate = DateTime.Now, DOB = DateTime.Parse("1998-04-05"), Gender = "Female", Phone = Convert.ToDouble("6179430772"), Role = "Functional Consultant" }
            };
            foreach (Employee e in employees)
            {
                context.Employees.Add(e);
            }
            context.SaveChanges();
        }
    }
}
