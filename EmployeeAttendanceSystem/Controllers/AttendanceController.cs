using ClosedXML.Excel;
using EmployeeAttendanceSystem.Data;
using EmployeeAttendanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeAttendanceSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly EASContext _context;

        public AttendanceController(EASContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            return _context.Attendance != null ? 
                          View(await _context.Attendance.Where(a=>a.Date.Date == DateTime.Now.Date).ToListAsync()) :
                          Problem("Entity set 'EASContext.Attendance'  is null.");
        }
        public async Task<IActionResult> Report(string sortOrder, string searchString)
        {
            var employees = await _context.Employees.ToListAsync();
            var attendance = await _context.Attendance.ToListAsync();
            var result = from e in employees
                         join a in attendance on e.EmployeeID equals a.EmployeeID
                         select new {a.AttendanceId, e.EmployeeID, e.FirstName, e.LastName, a.Date, a.Status };
            List<AttendanceReport> attendanceReports = new List<AttendanceReport>();
            foreach(var res in result) {
                AttendanceReport attendanceReport = new AttendanceReport();
                attendanceReport.EmployeeID = res.EmployeeID;
                attendanceReport.AttendanceId = res.AttendanceId;
                attendanceReport.LastName = res.LastName;
                attendanceReport.FirstName = res.FirstName;
                attendanceReport.Date = res.Date;
                attendanceReport.Status = res.Status;
                attendanceReports.Add(attendanceReport);
            }

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            //ViewData["ExportToExcel"] = String.IsNullOrEmpty(sortOrder) ? "export_to_excel" : "";
            var sortedReport = from s in attendanceReports select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                sortedReport = sortedReport.Where(s => s.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                       || s.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                       || s.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    sortedReport = sortedReport.OrderByDescending(s => s.FullName);
                    break;
                case "Date":
                    sortedReport = sortedReport.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    sortedReport = sortedReport.OrderByDescending(s => s.Date);
                    break;
                default:
                    sortedReport = sortedReport.OrderBy(s => s.FullName);
                    break;
            }
            
            return View(sortedReport);
        }

        [HttpPost]
        public ActionResult ExportToExcel()
        {
            var employees = _context.Employees.ToList();
            var attendance = _context.Attendance.ToList();
            var result = from e in employees
                         join a in attendance on e.EmployeeID equals a.EmployeeID
                         select new { a.AttendanceId, e.EmployeeID, e.FirstName, e.LastName, a.Date, a.Status };
            List<AttendanceReport> attendanceReports = new List<AttendanceReport>();
            foreach (var res in result)
            {
                AttendanceReport attendanceReport = new AttendanceReport();
                attendanceReport.EmployeeID = res.EmployeeID;
                attendanceReport.AttendanceId = res.AttendanceId;
                attendanceReport.LastName = res.LastName;
                attendanceReport.FirstName = res.FirstName;
                attendanceReport.Date = res.Date;
                attendanceReport.Status = res.Status;
                attendanceReports.Add(attendanceReport);
            }
            DataTable dt = new DataTable(DateTime.Now.Date.Date.ToLongDateString());
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Employee ID"),
                                        new DataColumn("Full Name"),
                                        new DataColumn("Date"),
                                        new DataColumn("Status") });

            foreach (var attendanceReport in attendanceReports)
            {
                dt.Rows.Add(attendanceReport.EmployeeID, attendanceReport.FullName, attendanceReport.Date, attendanceReport.Status);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance Report_"+ DateTime.Now.Date.Date.ToLongDateString()+".xlsx");
                }
            }
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Attendance == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendance
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceId,EmployeeID,Date,Status")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Attendance == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            return View(attendance);
        }

        // POST: Attendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceId,EmployeeID,Date,Status")] Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Report));
            }
            return View(attendance);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Attendance == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendance
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Attendance == null)
            {
                return Problem("Entity set 'EASContext.Attendance'  is null.");
            }
            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendance.Remove(attendance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Report));
        }

        private bool AttendanceExists(int id)
        {
          return (_context.Attendance?.Any(e => e.AttendanceId == id)).GetValueOrDefault();
        }
    }
}
