using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Students;
using STUDENT_MANAGEMENT.Models;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/students - Database se tamam students fetch karna
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Database ke Students table se sara data asynchronously lana
            var students = await _context.Students.ToListAsync();

            // 200 OK status ke sath data return karna
            return Ok(students);
        }
        // GET: api/students/{id} - Kisi aik student ko uski ID se get karna
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Database me ID match karne wala student dhoondna
            var student = await _context.Students.FindAsync(id);

            if(student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }
        // POST: api/students - Naya student DTO ke zariye create karna
        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDTO dto)
        {
            // Duplicate Email Check (Section 9.1 rule)
            var emailExists = await _context.Students.AnyAsync(s => s.Email == dto.Email);
            if (emailExists)
            {
                return Conflict("A student with this email already exists.");
            }

            // DTO ko database entity (Student) mein map karna
            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            // Response DTO banana
            var response = new StudentResponseDTO
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                Phone = student.Phone,
                CreatedAt = student.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        // PUT: api/students/{id} - Student data ko UpdateStudentDTO se update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStudentDTO dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Check duplicate email (agar user ne email change ki hai aur wo kisi aur student ki hai)
            var emailExists = await _context.Students
                .AnyAsync(s => s.Email == dto.Email && s.Id != id);

            if (emailExists)
            {
                return Conflict("Another student is already using this email.");
            }

            // DTO ki values ko student entity mein update karna
            student.Name = dto.Name;
            student.Age = dto.Age;
            student.Email = dto.Email;
            student.Phone = dto.Phone;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/students/{id} - Student ko database se delete karna
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/students/search?name=Ali - Name ke zariye students search karna
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            // Check karna ke search ka word khali na ho
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }
            var students = await _context.Students
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return Ok(students);
        }
        // GET: api/students/filter-by-age?minAge=18&maxAge=25 - Age range ke mutabiq filter karna
        [HttpGet("filter-by-age")]
        public async Task<IActionResult> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            // Validation: minAge hamesha maxAge se choti ya barabar honi chahiye
            if (minAge > maxAge)
            {
                return BadRequest("minAge cannot be greater than maxAge.");
            }

            var students = await _context.Students
                .Where(s => s.Age >= minAge && s.Age <= maxAge)
                .ToListAsync();

            return Ok(students);
        }
        // GET: api/students/count - Total students ki tadad return karna
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            // Database me mojood total students count karna
            var totalCount = await _context.Students.CountAsync();

            return Ok(new { TotalStudents = totalCount });
        }
    }
}
