using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.Models;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/enrollments - Tamam enrollments student aur course details ke sath lana
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            return Ok(enrollments);
        }
        // GET: api/enrollments/{id} - Aik enrollment get karna
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return Ok(enrollment);
        }
        // POST: api/enrollments - Nayi enrollment create karna
        [HttpPost]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            // Rule 1: Check karein ke Student database mein exist karta hai ya nahi
            var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
            if (!studentExists)
            {
                return BadRequest("Invalid StudentId. Student does not exist.");
            }

            // Rule 2: Check karein ke Course database mein exist karta hai ya nahi
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);
            if (!courseExists)
            {
                return BadRequest("Invalid CourseId. Course does not exist.");
            }

            // Rule 3: Check karein ke student pehle se is course mein enrolled to nahi hai
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);

            if (alreadyEnrolled)
            {
                return Conflict("Student is already enrolled in this course.");
            }

            // Server side data set karna
            enrollment.EnrollmentDate = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(enrollment.Status))
            {
                enrollment.Status = "Active";
            }

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        // PUT: api/enrollments/{id} - Enrollment status update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            // Allowed statuses ki list
            var allowedStatuses = new[]
            {
                "Active","Completed", "Cancelled"
            };
            // Check karein ke status valid hai ya nahi
            if (!allowedStatuses.Contains(status))
            {
                return BadRequest("Invalid status. Allowed values are: Active, Completed, Cancelled.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            enrollment.Status = status;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // DELETE: api/enrollments/{id} - Enrollment delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if(enrollment == null)
            {
                return NotFound();
            }
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
