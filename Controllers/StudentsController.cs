using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
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
        // POST: api/students - Naya student database me create karna
        [HttpPost]
        public async Task<IActionResult>Create(Student student)
        {
            // Server side record creation ka waqt set karna
            student.CreatedAt = DateTime.UtcNow;

            // Database ke Students table me naya record add karna
            await _context.AddAsync(student);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = student.Id }, student);
        }
        // PUT: api/students/{id} - Mojooda student ka data update karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Student updatedStudent)
        {
            // PUT: api/students/{id} - Mojooda student ka data update karna
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Email = updatedStudent.Email;
            student.Phone = updatedStudent.Phone;

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
    }
}
