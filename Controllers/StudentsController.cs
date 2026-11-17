using Microsoft.AspNetCore.Mvc;
using STUDENT_MANAGEMENT.DTOs.Students;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/students - Tamam students fetch karna
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        // GET: api/students/{id} - Kisi aik student ko uski ID se get karna
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        // POST: api/students - Naya student create karna
        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDTO dto)
        {
            var createdStudent = await _studentService.CreateStudentAsync(dto);
            if (createdStudent == null)
            {
                return Conflict("A student with this email already exists.");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, createdStudent);
        }

        // PUT: api/students/{id} - Student update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStudentDTO dto)
        {
            var result = await _studentService.UpdateStudentAsync(id, dto);

            if (result == false)
            {
                return NotFound();
            }

            if (result == null)
            {
                return Conflict("Another student is already using this email.");
            }

            return NoContent();
        }

        // DELETE: api/students/{id} - Student delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/students/search?name=Ali - Name se search karna
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var students = await _studentService.SearchByNameAsync(name);
            return Ok(students);
        }

        // GET: api/students/filter-by-age?minAge=18&maxAge=25 - Age range se filter karna
        [HttpGet("filter-by-age")]
        public async Task<IActionResult> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            if (minAge > maxAge)
            {
                return BadRequest("minAge cannot be greater than maxAge.");
            }

            var students = await _studentService.FilterByAgeAsync(minAge, maxAge);
            return Ok(students);
        }

        // GET: api/students/count - Total students count return karna
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _studentService.GetTotalCountAsync();
            return Ok(new { TotalStudents = count });
        }
    }
}
