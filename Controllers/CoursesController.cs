using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Courses;
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
        // POST: api/courses - Naya course CreateCourseDTO ke zariye create karna
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDTO dto)
        {
            // Duplicate Course Name Check (Section 9.2 rule)
            var courseExists = await _context.Courses
                .AnyAsync(c => c.CourseName.ToLower() == dto.CourseName.ToLower());

            if (courseExists)
            {
                return Conflict("A course with this name already exists.");
            }

            // DTO ko Database Model (Course) mein map karna
            var course = new Course
            {
                CourseName = dto.CourseName,
                Description = dto.Description,
                DurationInMonths = dto.DurationInMonths,
                Fee = dto.Fee
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Response DTO banana
            var response = new CourseResponseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Description = course.Description,
                DurationInMonths = course.DurationInMonths,
                Fee = course.Fee
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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
