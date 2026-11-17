using Microsoft.AspNetCore.Mvc;
using STUDENT_MANAGEMENT.DTOs.Courses;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/courses - Tamam courses lana
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // GET: api/courses/{id} - Course by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        // POST: api/courses - Naya course create karna
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDTO dto)
        {
            var createdCourse = await _courseService.CreateCourseAsync(dto);
            if (createdCourse == null)
            {
                return Conflict("A course with this name already exists.");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdCourse.Id }, createdCourse);
        }

        // PUT: api/courses/{id} - Course update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCourseDTO dto)
        {
            var result = await _courseService.UpdateCourseAsync(id, dto);

            if (result == false)
            {
                return NotFound();
            }

            if (result == null)
            {
                return Conflict("Another course with this name already exists.");
            }

            return NoContent();
        }

        // DELETE: api/courses/{id} - Course delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _courseService.DeleteCourseAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
