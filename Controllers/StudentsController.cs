using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;

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
    }
}
