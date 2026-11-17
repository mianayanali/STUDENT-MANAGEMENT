using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Enrollments;
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
                .Select(e => new EnrollmentResponseDTO
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.Name : string.Empty,
                    CourseId = e.CourseId,
                    CourseName = e.Course != null ? e.Course.CourseName : string.Empty,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
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
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentResponseDTO
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.Name : string.Empty,
                    CourseId = e.CourseId,
                    CourseName = e.Course != null ? e.Course.CourseName : string.Empty,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            return Ok(enrollment);
        }

        // POST: api/enrollments - Nayi enrollment create karna DTO ke zariye
        [HttpPost]
        public async Task<IActionResult> Create(CreateEnrollmentDTO dto)
        {
            // Rule 1: Check karein ke Student database mein exist karta hai ya nahi
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
            {
                return BadRequest("Invalid StudentId. Student does not exist.");
            }

            // Rule 2: Check karein ke Course database mein exist karta hai ya nahi
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return BadRequest("Invalid CourseId. Course does not exist.");
            }

            // Rule 3: Check karein ke student pehle se is course mein enrolled to nahi hai
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);

            if (alreadyEnrolled)
            {
                return Conflict("Student is already enrolled in this course.");
            }

            // DTO ko database entity mein convert karna
            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            // Response DTO banana
            var response = new EnrollmentResponseDTO
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.Name,
                CourseId = enrollment.CourseId,
                CourseName = course.CourseName,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT: api/enrollments/{id} - Enrollment status update karna UpdateEnrollmentDTO se
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateEnrollmentDTO dto)
        {
            // Allowed statuses ki list
            var allowedStatuses = new[] { "Active", "Completed", "Cancelled" };

            if (!allowedStatuses.Contains(dto.Status))
            {
                return BadRequest("Invalid status. Allowed values are: Active, Completed, Cancelled.");
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/enrollments/{id} - Enrollment delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
