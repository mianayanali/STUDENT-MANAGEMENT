using Microsoft.AspNetCore.Mvc;
using STUDENT_MANAGEMENT.DTOs.Enrollments;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/enrollments - Tamam enrollments lana
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

        // GET: api/enrollments/{id} - Enrollment by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return Ok(enrollment);
        }

        // POST: api/enrollments - Nayi enrollment create karna
        [HttpPost]
        public async Task<IActionResult> Create(CreateEnrollmentDTO dto)
        {
            var (response, error) = await _enrollmentService.CreateEnrollmentAsync(dto);

            if (error == "DUPLICATE")
            {
                return Conflict("Student is already enrolled in this course.");
            }

            if (error != null)
            {
                return BadRequest(error);
            }

            return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
        }

        // PUT: api/enrollments/{id} - Status update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateEnrollmentDTO dto)
        {
            var result = await _enrollmentService.UpdateEnrollmentStatusAsync(id, dto);

            if (result == null)
            {
                return BadRequest("Invalid status. Allowed values are: Active, Completed, Cancelled.");
            }

            if (result == false)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/enrollments/{id} - Enrollment delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
