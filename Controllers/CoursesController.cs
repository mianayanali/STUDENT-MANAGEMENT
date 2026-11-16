using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.Models;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/courses - Tamam courses database se lana
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }
        // GET: api/courses/{id} - Kisi aik course ko uski ID se dhoondna
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
        // POST: api/courses - Naya course database me create karna
        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            // Business Validation Rules
            if (string.IsNullOrWhiteSpace(course.CourseName))
            {
                return BadRequest("Course name is required.");

            }
            if(course.DurationInMonths <= 0)
            {
                return BadRequest("Duration must be greater than zero months.");
            }
            if (course.Fee < 0)
            {
                return BadRequest("Fee cannot be negative.");
            }
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }
        // PUT: api/courses/{id} - Mojooda course ka data update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Course updatedCourse)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(updatedCourse.CourseName))
            {
                return BadRequest("Course name is required.");
            }

            if (updatedCourse.DurationInMonths <= 0)
            {
                return BadRequest("Duration must be greater than zero months.");
            }

            if (updatedCourse.Fee < 0)
            {
                return BadRequest("Fee cannot be negative.");
            }

            // Database se course dhoondna
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            // Fields update karna
            course.CourseName = updatedCourse.CourseName;
            course.Description = updatedCourse.Description;
            course.DurationInMonths = updatedCourse.DurationInMonths;
            course.Fee = updatedCourse.Fee;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/courses/{id} - Course ko database se delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
